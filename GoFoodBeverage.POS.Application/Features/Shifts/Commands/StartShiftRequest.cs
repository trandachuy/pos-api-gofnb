using System;
using MediatR;
using MoreLinq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.POS.Application.Features.Shifts.Commands
{
    public class StartShiftRequest : IRequest<string>
    {
        public Guid BranchId { get; set; }

        public decimal InitialAmount { get; set; }

        public DateTime CheckInDateTime { get; set; }

        public List<MaterialInventoryCheckingDto> MaterialInventoryCheckings { get; set; }

        public class MaterialInventoryCheckingDto
        {
            public Guid? Id { get; set; }

            public int Quantity { get; set; }

            public int InventoryQuantity { get; set; }

            public string Reason { get; set; }
        }
    }

    public class StartShiftRequestHandler : IRequestHandler<StartShiftRequest, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTService _jwtService;
        private readonly IUserProvider _userProvider;
        private readonly IStaffService _staffService;

        public StartShiftRequestHandler(
            IUnitOfWork unitOfWork,
            IJWTService jwtService,
            IStaffService staffService,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _staffService = staffService;
            _userProvider = userProvider;
        }

        public async Task<string> Handle(StartShiftRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(loggedUser.AccountId.Value).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(staff == null, "Cannot find staff information");

            var branch = await _unitOfWork.StoreBranches
                .Find(b => b.StoreId == loggedUser.StoreId && b.Id == request.BranchId && !b.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(branch == null, "Cannot find branch information");

            var totalRecords = await _unitOfWork.Shifts.GetAllShiftInBranch(branch.Id).AsNoTracking().CountAsync();

            var startShift = await StartShift(request, loggedUser.StoreId.Value, staff.Id, totalRecords);

            var account = await _unitOfWork.Accounts.Find(a => a.Id == loggedUser.AccountId.Value)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(account == null, "Cannot find account information");

            string tokenUpdated = await UpdateToken(request.CheckInDateTime, loggedUser.StoreId.Value, request.BranchId, account, startShift.Id);

            if(request.MaterialInventoryCheckings?.Count > 0)
            {
                var materialInventoryCheckings = await UpdateMaterialInventoryChecking(loggedUser.StoreId.Value, request.BranchId, staff.Id, startShift.Id, request);
                _unitOfWork.MaterialInventoryCheckings.AddRange(materialInventoryCheckings);
            }

            _unitOfWork.Shifts.Add(startShift);
            await _unitOfWork.SaveChangesAsync();

            return tokenUpdated;
        }

        public async Task<Shift> StartShift(StartShiftRequest request, Guid storeId, Guid staffId, int totalRecords)
        {
            var shift = _unitOfWork.Shifts
                .Find(s => s.StoreId == storeId && s.BranchId == request.BranchId && s.CheckOutDateTime == null)
                .OrderByDescending(s => s.CheckInDateTime).FirstOrDefault();

            if(shift != null)
            {
                await _staffService.EndShiftAsync(shift.Id);
            }

            var addNewShift = new Shift()
            {
                Code = $"{DefaultConstants.SHIFT_CODE}-{totalRecords + 1:0000}",
                StoreId = storeId,
                StaffId = staffId,
                BranchId = request.BranchId,
                InitialAmount = request.InitialAmount,
                CheckInDateTime = request.CheckInDateTime
            };

            return addNewShift;
        }

        private async Task<string> UpdateToken(DateTime checkInDateTime, Guid storeId, Guid branchId, Account account, Guid shiftId)
        {

            var user = new LoggedUserModel();
            var fullName = string.Empty;
            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(account.Id).AsNoTracking().FirstOrDefaultAsync();
            if (staff == null)
            {
                var customer = await _unitOfWork.Customers.Find(s => s.AccountId == account.Id).AsNoTracking().FirstOrDefaultAsync();
                fullName = $"{customer?.FirstName} {customer?.LastName}";
                user.Id = customer.Id;
            }
            else
            {
                fullName = staff?.FullName;
                user.Id = staff.Id;
            }

            user.AccountId = account.Id;
            user.StoreId = storeId;
            user.BranchId = branchId;
            user.ShiftId = shiftId;
            user.UserName = account.Username;
            user.FullName = fullName;
            user.Email = account.Username;
            user.Password = account.Password;
            user.AccountTypeId = account.AccountTypeId;
            user.AccountType = account.AccountType.Title;
            user.IsStartShift = false;
            user.LoginDateTime = checkInDateTime;

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(staff.StoreId);
            if (store != null)
            {
                user.CurrencyCode = store.Currency?.Code;
                user.CurrencySymbol = store.Currency?.Symbol;
            }

            var accessToken = _jwtService.GeneratePOSAccessToken(user);

            return accessToken;
        }

        private async Task<List<MaterialInventoryChecking>> UpdateMaterialInventoryChecking(Guid storeId, Guid branchId, Guid staffId, Guid shiftId, StartShiftRequest request)
        {
            var materialInventoryCheckings = new List<MaterialInventoryChecking>();

            var materialIds = request.MaterialInventoryCheckings.Select(p => p.Id);
            var materials = _unitOfWork.Materials
                .GetAllMaterialsInStore(storeId)
                .Where(x => materialIds.Contains(x.Id));

            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches.GetMaterialInventoryBranchesByBranchId(storeId, branchId).ToListAsync();

            request.MaterialInventoryCheckings.ForEach(m =>
            {
                var materialInventoryChecking = new MaterialInventoryChecking()
                {
                    StoreId = storeId,
                    BranchId = branchId,
                    StaffId = staffId,
                    ShiftId = shiftId,
                    MaterialId = m.Id,
                    OriginalQuantity = m.Quantity,
                    InventoryQuantity = m.InventoryQuantity,
                    Reason = m.Reason
                };
                materialInventoryCheckings.Add(materialInventoryChecking);
            });

            materials.ForEach(material =>
            {
                var itemMaterial = request.MaterialInventoryCheckings.FirstOrDefault(item => item.Id == material.Id);
                var itemMaterialInventoryBranch = request.MaterialInventoryCheckings.FirstOrDefault(m => m.Id == material.Id);
                if (itemMaterial != null)
                {
                    material.Quantity = (material.Quantity + itemMaterial.InventoryQuantity) - itemMaterialInventoryBranch.Quantity;
                }
            });
            _unitOfWork.Materials.UpdateRange(materials);

            materialInventoryBranches.ForEach(mib =>
            {
                var itemMaterialInventoryBranch = request.MaterialInventoryCheckings.FirstOrDefault(m => m.Id == mib.MaterialId);
                if (itemMaterialInventoryBranch != null)
                {
                    mib.Quantity = itemMaterialInventoryBranch.InventoryQuantity;
                }
            });
            _unitOfWork.MaterialInventoryBranches.UpdateRange(materialInventoryBranches);

            return materialInventoryCheckings;
        }
    }
}

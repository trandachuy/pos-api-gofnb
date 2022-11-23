using System;
using MediatR;
using MoreLinq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.POS.Application.Features.Shifts.Commands
{
    public class EndShiftRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public DateTime CheckOutDateTime { get; set; }

        public List<MaterialInventoryCheckingDto> MaterialInventoryCheckings { get; set; }

        public class MaterialInventoryCheckingDto
        {
            public Guid? Id { get; set; }

            public int Quantity { get; set; }

            public int InventoryQuantity { get; set; }

            public string Reason { get; set; }
        }
    }

    public class EndShiftRequestHandler : IRequestHandler<EndShiftRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public EndShiftRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(EndShiftRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var shift = await _unitOfWork.Shifts.Find(b => b.StoreId == loggedUser.StoreId && b.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(shift == null, "Cannot find shift information");

            var endShift = EndShift(shift, request);

            if (request.MaterialInventoryCheckings?.Count > 0)
            {
                var materialInventoryCheckings = await UpdateMaterialInventoryChecking(loggedUser.StoreId.Value, shift.BranchId.Value, shift.StaffId.Value, shift.Id, request);
                _unitOfWork.MaterialInventoryCheckings.AddRange(materialInventoryCheckings);
            }

            _unitOfWork.Shifts.Update(endShift);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public static Shift EndShift(Shift shift, EndShiftRequest request)
        {
            shift.WithdrawalAmount = request.WithdrawalAmount;
            shift.CheckOutDateTime = request.CheckOutDateTime;

            return shift;
        }

        private async Task<List<MaterialInventoryChecking>> UpdateMaterialInventoryChecking(Guid storeId, Guid branchId, Guid staffId, Guid shiftId, EndShiftRequest request)
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

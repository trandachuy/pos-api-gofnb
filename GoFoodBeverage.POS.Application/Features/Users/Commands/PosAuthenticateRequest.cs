using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Users.Commands
{
    public class PosAuthenticateRequest : IRequest<PosAuthenticateResponse>
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public Guid? BranchId { get; set; }
    }

    public class PosAuthenticateResponse
    {
        public string Token { get; set; }

        public IEnumerable<StoreDto> Stores { get; set; }

        public class StoreDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Thumbnail { get; set; }

            public IEnumerable<BranchDto> Branches { get; set; }
        }

        public class BranchDto : StoreDto { }
    }

    public class Handler : IRequestHandler<PosAuthenticateRequest, PosAuthenticateResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTService _jwtService;
        private readonly IDateTimeService _dateTimeService;
        private readonly MapperConfiguration _mapperConfiguration;

        public Handler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IJWTService jwtService,
            IDateTimeService dateTimeService,
            MapperConfiguration mapperConfiguration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _dateTimeService = dateTimeService;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<PosAuthenticateResponse> Handle(PosAuthenticateRequest request, CancellationToken cancellationToken)
        {
            ThrowError.Against(string.IsNullOrWhiteSpace(request.UserName), new JObject()
            {
                { "loginError", "signIn.pleaseInputYourUsername" },
            });

            ThrowError.Against(string.IsNullOrEmpty(request.Password), new JObject()
            {
                { "loginError", "signIn.pleaseInputYourPassword" },
            });

            var passwordHasher = new PasswordHasher<Domain.Entities.Account>();
            var listAccountByUserName = await _unitOfWork.Accounts
                .Find(a => a.Username.Trim().ToLower() == request.UserName.Trim().ToLower())
                .Include(a => a.AccountType)
                .ToListAsync(cancellationToken: cancellationToken);

            var listAccountVerified = new List<Domain.Entities.Account>();
            foreach (var account in listAccountByUserName)
            {
                var verified = passwordHasher.VerifyHashedPassword(null, account.Password, request.Password);
                if (verified == PasswordVerificationResult.Success)
                {
                    listAccountVerified.Add(account);
                }
            }

            ThrowError.Against(!listAccountVerified.Any(), new JObject()
            {
                { "loginError", "signIn.loginFail" },
            });

            // admin
            var storeBranches = _unitOfWork.Stores
                .Find(store => listAccountVerified.Select(a => a.Id).Contains(store.InitialStoreAccountId))
                .AsNoTracking()
                .Include(store => store.StoreBranches)
                .SelectMany(store => store.StoreBranches);

            // staff has POS permission on branches
            var listStaffBranch = _unitOfWork.Staffs
                .Find(s => listAccountVerified.Select(a => a.Id).Contains(s.AccountId))
                .AsNoTracking()
                .Include(staff => staff.Store)
                .Include(staff => staff.StaffGroupPermissionBranchs)
                    .ThenInclude(i => i.GroupPermissionBranches)
                    .ThenInclude(gr => gr.StoreBranch)
                .SelectMany(s => s.StaffGroupPermissionBranchs)
                .SelectMany(i => i.GroupPermissionBranches)
                .Select(gr => new
                {
                    StoreBranch = gr.StoreBranch,
                    Permissions = gr.GroupPermission.GroupPermissionPermissions.Select(i => i.Permission).Where(p => p.PermissionGroupId == EnumPermissionGroup.POS.ToGuid()),
                })
                .Where(gr => gr.Permissions.Any()).Select(gr => gr.StoreBranch);

            var branches = await storeBranches.Concat(listStaffBranch)
                .Where(b => b.IsDeleted != true)
                .Select(branch => new
                {
                    Id = branch.Id,
                    BranchName = branch.Name,
                    StoreId = branch.StoreId,
                    StoreName = branch.Store.Title,
                    Thumbnail = branch.Store.Logo
                })
                .ToListAsync(cancellationToken: cancellationToken);

            ThrowError.Against(!branches.Any(), new JObject()
            {
                { "loginError", "signIn.permissionDenied" },
            });

            var response = new PosAuthenticateResponse();
            if (request.BranchId.HasValue)
            {
                // get account for branch

                response.Token = await CreateToken(request, listAccountVerified, request.BranchId);
            }
            else
            {
                response.Stores = branches.GroupBy(p => p.StoreId, (key, g) => new PosAuthenticateResponse.StoreDto()
                {
                    Id = key,
                    Name = g.FirstOrDefault()?.StoreName,
                    Thumbnail = g.FirstOrDefault()?.Thumbnail,
                    Branches = g.Select(b => new PosAuthenticateResponse.BranchDto()
                    {
                        Id = b.Id,
                        Name = b.BranchName
                    })
                });
            }

            return response;
        }

        private bool ValidataPermission(Guid accountId, Guid storeId, Guid staffId)
        {
            bool result = true;
            var isStaffInitStore = _unitOfWork.Stores.IsStaffInitStore(accountId, storeId);
            /// If is initial store set full permission
            if (!isStaffInitStore)
            {
                var staffGroupPermissionBranches = _unitOfWork.StaffGroupPermissionBranches
                .GetStaffGroupPermissionBranchesByStaffId(staffId)
                .AsNoTracking()
                .ToList();
                var groupPermissions = staffGroupPermissionBranches
                .SelectMany(s => s.GroupPermissionBranches.Select(g => g.GroupPermission))
                .DistinctBy(g => g.Id);

                var groupPermissionIds = groupPermissions.Select(g => g.Id);
                /// Get permissions
                var groupPermissionPermissions = _unitOfWork.GroupPermissionPermissions
                    .Where(g => groupPermissionIds.Any(gpid => gpid == g.GroupPermissionId))
                    .AsNoTracking()
                    .Include(g => g.Permission)
                    .ToList();
                var permissions = groupPermissionPermissions.Select(g => g.Permission);
                var permissionsDisticted = permissions.DistinctBy(p => p.Id).ToList();
                result = permissionsDisticted.Where(x => x.PermissionGroupId == EnumPermissionGroup.POS.ToGuid()).Any();
            }

            return result;
        }

        private async Task<string> CreateToken(PosAuthenticateRequest request, List<Domain.Entities.Account> listAccountVerified, Guid? branchId)
        {
            var store = await _unitOfWork.StoreBranches
                .Where(s => s.Id == branchId)
                .Include(s => s.Store).ThenInclude(s => s.Currency)
                .AsNoTracking()
                .Select(s => new
                {
                    StoreId = s.StoreId,
                    InitialStoreAccountId = s.Store.InitialStoreAccountId,
                    CurrencyCode = s.Store.Currency.Code,
                    CurrencySymbol = s.Store.Currency.Symbol,
                })
                .FirstOrDefaultAsync();

            var staff = new Domain.Entities.Staff()
            {
                Id = Guid.Empty
            };

            var user = new LoggedUserModel();
            var account = listAccountVerified.FirstOrDefault(acc => store.InitialStoreAccountId == acc.Id);
            if (account == null)
            {
                /// Staff
                var accountIds = listAccountVerified.Select(acc => acc.Id);
                staff = await _unitOfWork.Staffs
                    .Where(s => accountIds.Contains(s.Id))
                    .Include(s => s.Account).ThenInclude(account => account.AccountType)
                    .Include(s => s.StaffGroupPermissionBranchs)
                        .ThenInclude(sgpb => sgpb.GroupPermissionBranches)
                    .AsNoTracking()
                    .Select(s => new
                    {
                        s.Id,
                        s.AccountId,
                        s.FullName,
                        s.Email,
                        s.Account,
                        GroupPermissionBranches = s.StaffGroupPermissionBranchs.SelectMany(i => i.GroupPermissionBranches)
                    })
                    .Where(s => s.GroupPermissionBranches.Any())
                    .Select(s => new Domain.Entities.Staff
                    {
                        Id = s.Id,
                        AccountId = s.AccountId,
                        FullName = s.FullName,
                        Email = s.Email,
                        Account = s.Account,
                    })
                    .FirstOrDefaultAsync();

            }
            else
            {
                /// Admin
                staff = await _unitOfWork.Staffs
                    .Where(s => s.AccountId == account.Id)
                    .Include(s => s.Account).ThenInclude(a => a.AccountType)
                    .AsNoTracking()
                    .Select(s => new Domain.Entities.Staff
                    {
                        Id = s.Id,
                        AccountId = s.AccountId,
                        FullName = s.FullName,
                        Email = s.Email,
                        Account = s.Account,
                    })
                    .FirstOrDefaultAsync();
            }

            ThrowError.BadRequestAgainstNull(staff, "Cannot find your account information.");

            var userName = staff.Account.Username;
            user.Id = staff.Id;
            user.AccountId = staff.AccountId;
            user.StoreId = store.StoreId;
            user.BranchId = request.BranchId;
            user.UserName = userName;
            user.Password = account.Password;
            user.FullName = staff?.FullName;
            user.Email = staff.Email;
            user.AccountTypeId = staff.Account.AccountTypeId;
            user.AccountType = staff.Account.AccountType.Title;
            user.IsStartShift = true;
            user.LoginDateTime = _dateTimeService.NowUtc;

            if (store != null)
            {
                user.CurrencyCode = store.CurrencyCode;
                user.CurrencySymbol = store.CurrencySymbol;
            }

            var accessToken = _jwtService.GeneratePOSAccessToken(user);

            return accessToken;
        }
    }
}

using System;
using MediatR;
using MoreLinq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class Authenticate
    {
        public class Request : IRequest<Response>
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public Guid StoreId { get; set; }

            public Guid AccountId { get; set; }
        }

        public class Response
        {
            public string Token { get; set; }

            public string AccountStatus { get; set; }

            public string Thumbnail { get; set; }

            public string StoreLogoUrl { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IJWTService _jwtService;

            public Handler(IUnitOfWork unitOfWork, IJWTService jwtService)
            {
                _unitOfWork = unitOfWork;
                _jwtService = jwtService;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                ThrowError.ArgumentIsNull(request, request.UserName);
                ThrowError.ArgumentIsNull(request, request.Password);

                var hasher = new PasswordHasher<Domain.Entities.Account>();
                var accountEntity = await _unitOfWork.Accounts
                    .Find(a => a.Id == request.AccountId && a.Username == request.UserName.Trim().ToLower() && !a.IsDeleted)
                    .Include(a => a.AccountType)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken: cancellationToken);
                var account = new Domain.Entities.Account();
                var verified = hasher.VerifyHashedPassword(null, accountEntity.Password, request.Password);
                if (verified == PasswordVerificationResult.Success)
                {
                    account = accountEntity;
                }

                ThrowError.Against(account == null || account.Username == null, "signIn.errorLogin");

                var user = new LoggedUserModel();
                var fullName = string.Empty;
                var staff = await _unitOfWork.Staffs
                    .Find(s => s.AccountId == account.Id)
                    .AsNoTracking()
                    .Select(a => new Staff()
                    {
                        Id = a.Id,
                        Thumbnail = a.Thumbnail,
                        FullName = a.FullName,
                        PhoneNumber = a.PhoneNumber,
                        Store = new Store()
                        {
                            Id = a.Store.Id,
                            Logo = a.Store.Logo
                        }
                    })
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                ThrowError.Against(staff == null, "signIn.errorLogin");
                ThrowError.Against(!(await ValidatePermission(account.Id, staff.Id, request.StoreId)), "signIn.permissionDenied");

                if (staff == null)
                {
                    var customer = await _unitOfWork.Customers
                        .Find(s => s.AccountId == account.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                    fullName = $"{customer?.FirstName} {customer?.LastName}";
                    user.Id = customer.Id;
                }
                else
                {
                    fullName = staff?.FullName;
                    user.Id = staff.Id;
                }

                user.AccountId = account.Id;
                user.StoreId = request.StoreId;
                user.UserName = account.Username;
                user.FullName = fullName;
                user.Email = account.Username;
                user.Password = account.Password;
                user.AccountTypeId = account.AccountTypeId;
                user.AccountType = account.AccountType.Title;
                user.PhoneNumber = staff.PhoneNumber;

                var isStaffInitStore = await _unitOfWork.Stores.IsStaffInitStoreAsync(account.Id, request.StoreId);
                if (isStaffInitStore)
                {
                    user.AccountType = DefaultConstants.ADMIN_ACCOUNT;
                }

                var store = await _unitOfWork.Stores.GetStoreByIdAsync(request.StoreId);
                if (store != null)
                {
                    user.CurrencyCode = store.Currency?.Code;
                    user.CurrencySymbol = store.Currency?.Symbol;
                    user.StoreName = store.Title;
                }

                var response = new Response() { Thumbnail = staff.Thumbnail, StoreLogoUrl = staff.Store.Logo };
                response.Token = _jwtService.GenerateAccessToken(user);
                if (store.IsActivated)
                {
                    return response;
                }

                var waitingForApproval = await _unitOfWork.OrderPackages
                    .Find(o => o.StoreId == store.Id)
                    .AsNoTracking()
                    .AnyAsync(o => o.ExpiredDate == null && o.Status == EnumOrderPackageStatus.PENDING.GetName(), cancellationToken: cancellationToken);

                if (waitingForApproval)
                {
                    return new Response() { AccountStatus = AccountStatusConstants.WAITING_FOR_APPROVAL };
                }

                response.AccountStatus = AccountStatusConstants.FIRST_LOGIN;

                return response;
            }

            private async Task<bool> ValidatePermission(Guid accountId, Guid staffId, Guid storeId)
            {
                bool result = true;
                var isStaffInitStore = await _unitOfWork.Stores.IsStaffInitStoreAsync(accountId, storeId);
                /// If is initial store set full permission
                if (!isStaffInitStore)
                {
                    var staffGroupPermissionBranches = await _unitOfWork.StaffGroupPermissionBranches
                    .GetStaffGroupPermissionBranchesByStaffId(staffId)
                    .AsNoTracking()
                    .ToListAsync();
                    var groupPermissions = staffGroupPermissionBranches
                    .SelectMany(s => s.GroupPermissionBranches.Select(g => g.GroupPermission))
                    .DistinctBy(g => g.Id);

                    var groupPermissionIds = groupPermissions.Select(g => g.Id);
                    /// Get permissions
                    var groupPermissionPermissions = (await _unitOfWork.GroupPermissionPermissions
                        .Find(g => groupPermissionIds.Any(gpid => gpid == g.GroupPermissionId))
                        .AsNoTracking()
                        .Include(g => g.Permission)
                        .ToListAsync());
                    var permissions = groupPermissionPermissions.Select(g => g.Permission);
                    var permissionsDisticted = permissions.DistinctBy(p => p.Id).ToList();
                    result = permissionsDisticted.Where(x => x.PermissionGroupId != EnumPermissionGroup.POS.ToGuid()).Any();
                }

                return result;
            }

        }
    }
}
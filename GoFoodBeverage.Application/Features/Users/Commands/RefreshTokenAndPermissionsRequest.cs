using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Application.Features.Settings.Queries;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Permission;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Users.Commands
{
    public class RefreshTokenAndPermissionsRequest : IRequest<RefreshTokenAndPermissionsResponse>
    {
    }

    public class RefreshTokenAndPermissionsResponse
    {
        public string Token { get; set; }

        public IEnumerable<PermissionModel> Permissions { get; set; }
    }

    public class RefreshTokenAndPermissionsRequestHandler : IRequestHandler<RefreshTokenAndPermissionsRequest, RefreshTokenAndPermissionsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTService _jwtService;
        private readonly IUserProvider _userProvider;
        private readonly IMediator _mediator;

        public RefreshTokenAndPermissionsRequestHandler(IUnitOfWork unitOfWork, IJWTService jwtService, IUserProvider userProvider, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _userProvider = userProvider;
            _mediator = mediator;
        }

        public async Task<RefreshTokenAndPermissionsResponse> Handle(RefreshTokenAndPermissionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.AccountId.HasValue, "Cannot find account information");
            
            var account = await _unitOfWork.Accounts
                .Find(a => a.Id == loggedUser.AccountId)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            var user = new LoggedUserModel();
            var fullName = string.Empty;
            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(account.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (staff == null)
            {
                var customer = await _unitOfWork.Customers.Find(s => s.AccountId == account.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken: cancellationToken);
                fullName = $"{customer?.FirstName} {customer?.LastName}";
                user.Id = customer.Id;
            }
            else
            {
                fullName = staff?.FullName;
                user.Id = staff.Id;
            }

            user.AccountId = account.Id;
            user.StoreId = staff.StoreId;
            user.UserName = account.Username;
            user.FullName = fullName;
            user.Email = account.Username;
            user.Password = account.Password;
            user.AccountTypeId = account.AccountTypeId;
            user.AccountType = account.AccountType.Title;
            user.PhoneNumber = staff.PhoneNumber;

            var isStaffInitStore = await _unitOfWork.Stores.IsStaffInitStoreAsync(account.Id, staff.StoreId.Value);
            if (isStaffInitStore)
            {
                user.AccountType = DefaultConstants.ADMIN_ACCOUNT;
            }

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(staff.StoreId);
            if (store != null)
            {
                user.CurrencyCode = store.Currency?.Code;
                user.CurrencySymbol = store.Currency?.Symbol;
                user.StoreName = store.Title;
            }

            var accessToken = _jwtService.GenerateAccessToken(user);

            var permissions = new GetPermissionsRequest()
            {
                Token = accessToken
            };
            var permissionsResult = await _mediator.Send(permissions, cancellationToken);

            return new RefreshTokenAndPermissionsResponse()
            {
                Token = accessToken,
                Permissions = permissionsResult.Permissions
            };
        }
    }
}

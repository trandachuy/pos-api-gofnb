using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Services.User
{
    [AutoService(typeof(IUserService), Lifetime = ServiceLifetime.Scoped)]
    public class UserService : IUserService
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public bool PasswordValidation(string currentPassword, out Account account)
        {
            var loggerUser = _userProvider.Provide();
            if (loggerUser == null) ThrowError.Against(!loggerUser.AccountId.HasValue, "Cannot find account information");
            var hasher = new PasswordHasher<Account>();

            account = _unitOfWork.Accounts
                .Find(a => a.Id == loggerUser.AccountId)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync().Result;

            var verified = hasher.VerifyHashedPassword(null, account.Password, currentPassword);

            return verified == PasswordVerificationResult.Success;
        }

        #region GoApp

        public bool GoAppPasswordValidation(string currentPassword, out Account account)
        {
            var loggerUser = _userProvider.GetLoggedCustomer();
            if (loggerUser == null) ThrowError.Against(!loggerUser.AccountId.HasValue, "Cannot find account information");
            var hasher = new PasswordHasher<Account>();

            account = _unitOfWork.Accounts
                .Find(a => a.Id == loggerUser.AccountId)
                .Include(a => a.AccountType)
                .FirstOrDefaultAsync().Result;

            var verified = hasher.VerifyHashedPassword(null, account.Password, currentPassword);

            return verified == PasswordVerificationResult.Success;
        }

        #endregion
    }
}

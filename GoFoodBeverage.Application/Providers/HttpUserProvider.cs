using System.Linq;
using System.Threading;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Interfaces;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Providers
{
    public class HttpUserProvider : IUserProvider
    {
        private readonly ILogger<HttpUserProvider> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpUserProvider(
            ILogger<HttpUserProvider> logger,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<LoggedUserModel> ProvideAsync(CancellationToken cancellationToken = default)
        {
            var identifier = GetLoggedUserModelFromJwt(_httpContextAccessor.HttpContext.User);
            if (identifier == null)
            {
                _logger.LogWarning("object identifier is null for the user");
                throw new UnauthorisedException();
            }

            return Task.FromResult(identifier);
        }

        public LoggedUserModel Provide()
        {
            var identifier = GetLoggedUserModelFromJwt(_httpContextAccessor.HttpContext.User);
            if (identifier == null)
            {
                _logger.LogWarning("object identifier is null for the user");
                throw new UnauthorisedException();
            }

            return identifier;
        }

        public LoggedUserModel GetLoggedUserModelFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var id = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ID);
            var accountId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
            var storeId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.STORE_ID);
            var fullName = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.FULL_NAME);
            var email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.EMAIL);

            if (id == null) return null;

            ThrowError.Against(storeId == null, "Cannot find store information");

            var loggedUser = new LoggedUserModel()
            {
                Id = id.Value.ToGuid(),
                AccountId = accountId.Value.ToGuid(),
                StoreId = storeId.Value.ToGuid(),
                FullName = fullName.Value,
                Email = email?.Value
            };

            return loggedUser;
        }

        private static LoggedUserModel GetLoggedUserModelFromJwt(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ID);
            var accountId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.STORE_ID);
            var fullName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.FULL_NAME);
            var email = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.EMAIL);
            var phoneNumber = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.PHONE_NUMBER);

            if (id == null) return null;

            ThrowError.Against(storeId == null, "Cannot find store information");

            var loggedUser = new LoggedUserModel()
            {
                Id = id.Value.ToGuid(),
                AccountId = accountId.Value.ToGuid(),
                StoreId = storeId.Value.ToGuid(),
                FullName = fullName.Value,
                Email = email?.Value,
                PhoneNumber = phoneNumber?.Value
            };

            return loggedUser;
        }

        public string GetPlatformId()
        {
            var platform = _httpContextAccessor.HttpContext.Request.Headers[DefaultConstants.PLATFORM_ID];
            return platform;
        }

        #region Gofood App

        public LoggedUserModel GetLoggedCustomer()
        {
            var identifier = GetLoggedCustomerModelFromJwt(_httpContextAccessor.HttpContext.User);
            if (identifier == null)
            {
                _logger.LogWarning("object identifier is null for the user");
                throw new UnauthorisedException();
            }

            return identifier;
        }

        private static LoggedUserModel GetLoggedCustomerModelFromJwt(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ID);
            var accountId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
            var fullName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.FULL_NAME);
            var email = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.EMAIL);
            var phoneNumber = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.PHONE_NUMBER);
            var accountType = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_TYPE);

            if (id == null) return null;

            var loggedUser = new LoggedUserModel()
            {
                Id = id.Value.ToGuid(),
                AccountId = accountId.Value.ToGuid(),
                FullName = fullName.Value,
                Email = email?.Value,
                PhoneNumber = phoneNumber.Value,
                AccountType = accountType?.Value
            };

            return loggedUser;
        }

        #endregion
    }
}

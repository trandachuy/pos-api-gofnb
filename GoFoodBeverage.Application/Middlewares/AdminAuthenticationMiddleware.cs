using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.MemoryCaching;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Application.Middlewares
{
    public class AdminAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IJWTService jwtService,
            IAccountRepository accountRepository,
            IInternalAccountRepository internalAccountRepository,
            IMemoryCachingService memoryCachingService
        )
        {
            var authorization = context.Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization"));
            var token = authorization.Key == null ? string.Empty : context.Request.Headers["Authorization"].FirstOrDefault().Substring("Bearer".Length).Trim();

            if (!string.IsNullOrEmpty(token))
            {
                var isValid = IsValidUser(
                        jwtService,
                        memoryCachingService,
                        accountRepository,
                        internalAccountRepository,
                        token
                    );

                if (!isValid)
                {
                    var bytes = Encoding.UTF8.GetBytes("Unauthorized");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Headers.Add("Token-Expired", "true");
                    await context.Response.Body.WriteAsync(bytes.AsMemory(0, bytes.Length));

                    return;
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Check access token for store account or internal account
        /// </summary>
        /// <param name="jwtService"></param>
        /// <param name="memoryCachingService"></param>
        /// <param name="accountRepository"></param>
        /// <param name="internalAccountRepository"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsValidUser(
            IJWTService jwtService,
            IMemoryCachingService memoryCachingService,
            IAccountRepository accountRepository,
            IInternalAccountRepository internalAccountRepository,
            string token
        )
        {
            try
            {
                var isDataFromDatabase = false;
                var jwtToken = jwtService.ValidateToken(token);
                if (jwtToken == null) return false;

                #region Check store account
                var claimAccountId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
                var claimAccountTypeId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_TYPE);
                if (claimAccountId != null)
                {
                    if (claimAccountTypeId != null && claimAccountTypeId.Value == $"{(int)EnumAccountType.User}")
                    {
                        var customerInCaching = memoryCachingService.GetCache<Account>(token);
                        if (customerInCaching != null)
                        {
                            return true;
                        }
                        else
                        {
                            Guid customerId = Guid.Empty;
                            if (Guid.TryParse(claimAccountId.Value, out customerId))
                            {
                                var customer = accountRepository.GetAll().FirstOrDefault(x => x.Id == customerId);
                                if (customer != null)
                                {
                                    memoryCachingService.SetCache(token, customer);
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        var account = memoryCachingService.GetCache<Account>(token);
                        if (account == null)
                        {
                            var accountId = Guid.Parse(claimAccountId.Value);
                            account = accountRepository.GetIdentifier(accountId);
                            isDataFromDatabase = true;
                        }

                        if (account != null)
                        {
                            if (isDataFromDatabase) memoryCachingService.SetCache(token, account);

                            return true;
                        }
                    }

                }
                #endregion

                #region Check internal account
                var claimInternalAccountId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.INTERNAL_ACCOUNT_ID);
                if (claimInternalAccountId != null)
                {
                    var internalAccount = memoryCachingService.GetCache<Domain.Entities.InternalAccount>(token);
                    if (internalAccount == null)
                    {
                        var internalAccountId = Guid.Parse(claimInternalAccountId.Value);
                        internalAccount = internalAccountRepository.Find(i => i.Id == internalAccountId).FirstOrDefault();
                        isDataFromDatabase = true;
                    }

                    if (internalAccount != null)
                    {
                        if (isDataFromDatabase) memoryCachingService.SetCache(token, internalAccount);

                        return true;
                    }
                }


                #endregion

            }
            catch (Exception) { }

            return false;
        }
    }
}

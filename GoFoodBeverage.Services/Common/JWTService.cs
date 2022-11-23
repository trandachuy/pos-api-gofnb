using System;
using System.Security.Claims;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using GoFoodBeverage.Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Services
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtSettings;


        public JWTService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }


        /// <summary>
        /// This method is used to generate a new string of tokens to access our API(s).
        /// </summary>
        /// <returns>The token string</returns>
        public string GenerateAccessToken(LoggedUserModel user)
        {

            if ((string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password)) &&
                string.IsNullOrWhiteSpace(user.PhoneNumber)
             )
            {
                return null;
            }

            var tokenExpires = _jwtSettings.AccessTokenExpirationInMinutes;

            if (user.NeverExpires)
            {
                // 10 years.
                tokenExpires = 5256000;
            }

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Private Key to Encrypted
            var tokenKey = _jwtSettings.SecretBytes;

            // 3. Create JwtSecurityToken
            var claims = new List<Claim>
            {
                new Claim(ClaimTypesConstants.ID, user.Id.ToString()),
                new Claim(ClaimTypesConstants.ACCOUNT_ID, user.AccountId.ToString()),
                new Claim(ClaimTypesConstants.FULL_NAME, user.FullName)
            };

            if (!string.IsNullOrWhiteSpace(user.AccountType))
            {
                claims.Add(new Claim(ClaimTypesConstants.ACCOUNT_TYPE, user.AccountType));
            }

            if (user.StoreId.HasValue)
            {
                claims.Add(new Claim(ClaimTypesConstants.STORE_ID, $"{user.StoreId.Value}"));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypesConstants.EMAIL, user.Email));
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.Add(new Claim(ClaimTypesConstants.PHONE_NUMBER, user.PhoneNumber));
            }

            if (user.CountryId.HasValue)
            {
                claims.Add(new Claim(ClaimTypesConstants.COUNTRY_ID, $"{user.CountryId.Value}"));
            }

            if (!string.IsNullOrWhiteSpace(user.CountryCode))
            {
                claims.Add(new Claim(ClaimTypesConstants.COUNTRY_CODE, user.CountryCode));
            }

            if (!string.IsNullOrWhiteSpace(user.CurrencyCode))
            {
                claims.Add(new Claim(ClaimTypesConstants.CURRENCY_CODE, user.CurrencyCode));
            }

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
            var twtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpires),
                signingCredentials: signingCredentials);

            // 4. Return Token from method
            var jwtToken = tokenHandler.WriteToken(twtToken);
            return jwtToken;

        }

        /// <summary>
        /// GenerateAccessToken
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>AccessToken</returns>
        public string GeneratePOSAccessToken(LoggedUserModel user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return null;
            }

            DateTime loginDateTime, endDateTime;

            loginDateTime = user.LoginDateTime.ToLocalTime();
            endDateTime = loginDateTime.Date.AddDays(1).AddSeconds(-1);
            TimeSpan ts = endDateTime - loginDateTime;
            var tokenExpires = ts.TotalMinutes;

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Private Key to Encrypted
            var tokenKey = _jwtSettings.SecretBytes;

            //3. Create JwtSecurityToken
            var claims = new List<Claim>
            {
                new Claim(ClaimTypesConstants.ID, $"{ user.Id}"),
                new Claim(ClaimTypesConstants.ACCOUNT_ID, $"{user.AccountId}"),
                new Claim(ClaimTypesConstants.ACCOUNT_TYPE, user.AccountType),
                new Claim(ClaimTypesConstants.STORE_ID, $"{user.StoreId}"),
                new Claim(ClaimTypesConstants.BRANCH_ID, $"{user.BranchId}"),
                new Claim(ClaimTypesConstants.SHIFT_ID, $"{user.ShiftId}"),
                new Claim(ClaimTypesConstants.FULL_NAME, user.FullName),
                new Claim(ClaimTypesConstants.EMAIL, $"{user.Email}"),
                new Claim(ClaimTypesConstants.CURRENCY_CODE, user.CurrencyCode),
                new Claim(ClaimTypesConstants.CURRENCY_SYMBOL, user.CurrencySymbol),
                new Claim(ClaimTypesConstants.IS_START_SHIFT, $"{user.IsStartShift}"),

            };
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
            var twtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpires),
                signingCredentials: signingCredentials);

            // 4. Return Token from method
            var jwtToken = tokenHandler.WriteToken(twtToken);
            return jwtToken;
        }



        public string GenerateInternalToolAccessToken(LoggedUserModel user)
        {
            if ((string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password)) && string.IsNullOrWhiteSpace(user.PhoneNumber)
             )
            {
                return null;
            }

            var tokenExpires = _jwtSettings.AccessTokenExpirationInMinutes;

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Private Key to Encrypted
            var tokenKey = _jwtSettings.SecretBytes;

            // 3. Create JwtSecurityToken
            var claims = new List<Claim>
            {
                new Claim(ClaimTypesConstants.INTERNAL_ACCOUNT_ID, user.AccountId.ToString()),
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
            var twtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpires),
                signingCredentials: signingCredentials);

            // 4. Return Token from method
            var jwtToken = tokenHandler.WriteToken(twtToken);

            return jwtToken;
        }

        /// <summary>
        /// ValidateToken
        /// </summary>
        /// <param name="token"></param>
        /// <returns>JwtSecurityToken</returns>
        public JwtSecurityToken ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(_jwtSettings.SecretBytes);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}

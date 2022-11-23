using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GoFoodBeverage.Common.Extensions
{
    public static class SecurityExtensions
    {

        /// <summary>
        /// Description: this method is used to get the user id from the JSON web token.
        /// For example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiJlYzE4NTkxOC03ZWZhLTRkYjktOTEwZC01YzRlMWIzOGM2YTUiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdGF0aW9uQGJlZWNvdy50ZXN0IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IlNULTEgVMOibiBCw6xuaCBIQ00gc3RhZmYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJTVEFUSU9OIiwiY3VycmVudFN0YXRpb25JZCI6IjkzM2RjNDFkLWM2OTUtNDMwMC05M2M5LTFmZWM5OWI1N2RiYiIsIkNPTVBBTllfSURFTlRJVFkiOiJjNGYxZTBiMS1lMWRjLTRmMTEtYTk4Mi1lYzExMGM0YzA3MDMiLCJDSEFOR0VEX1BBU1NXT1JEIjoiVHJ1ZSIsImV4cCI6MTYzNTMwNzg0OSwiaXNzIjoiaHR0cHM6Ly9iZWVjb3cuY29tLyIsImF1ZCI6Imh0dHBzOi8vYmVlY293LmNvbS8ifQ.Hs4rNL-IpZ6JLM4RYfCDZehIHoMCEknqXlLl4NjmtxE"
        /// You can decode it at: https://jwt.io/
        /// </summary>
        /// <param name="claimsPrincipal">Please access to property: IHttpContextAccessor.HttpContext.User</param>
        /// <returns>The user id, for example: "98cedbfd-4128-48ac-b2db-c0688dedb149"</returns>
        public static Guid? GetUserIdFromJwt(this ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var userClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);
                if (userClaim != null)
                {
                    Guid guid = Guid.Empty;
                    string userIdString = userClaim.Value;

                    // Convert string type to guid type.
                    _ = Guid.TryParse(userIdString, out guid);

                    // Return data.
                    return guid;
                }
            }
            catch { }

            return null;

        }

        /// <summary>
        /// RSA Key Converter: PEM PublicKey to XML
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>xmlString</returns>
        public static string ConvertPublicKeyToXmlString(this string publicKey)
        {
            var pemPublicKey = "-----BEGIN PUBLIC KEY-----\n";
            pemPublicKey += $"{publicKey}";
            pemPublicKey += "\n";
            pemPublicKey += "-----END PUBLIC KEY-----";

            var rsa = RSA.Create();
            rsa.ImportFromPem(pemPublicKey.ToCharArray());
            var xmlString = rsa.ToXmlString(false);

            return xmlString;
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Interfaces
{
    public interface IJWTService
    {
        string GenerateAccessToken(LoggedUserModel user);

        string GeneratePOSAccessToken(LoggedUserModel user);

        JwtSecurityToken ValidateToken(string token);

        string GenerateInternalToolAccessToken(LoggedUserModel user);
    }
}

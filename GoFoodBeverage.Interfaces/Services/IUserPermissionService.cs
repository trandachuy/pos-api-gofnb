using GoFoodBeverage.Domain.Enums;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IUserPermissionService
    {
        Task<bool> CheckPermissionForUserAsync(ClaimsPrincipal claimsPrincipal, EnumPermission requirementPermission);
    }
}

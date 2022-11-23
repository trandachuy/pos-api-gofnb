using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace GoFoodBeverage.Common.Attributes.Permission
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(EnumPermission permission)
        {
            Permission = permission;
        }

        public EnumPermission Permission { get; }
    }
}

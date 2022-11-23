using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System;

namespace GoFoodBeverage.Common.Attributes.Permission
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(EnumPermission permission) : base(permission.ToString())
        {
        }
    }
}

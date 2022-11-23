using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Providers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserPermissionService _userPermissionService;

        public PermissionHandler(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (context.User.Identity.IsAuthenticated)
            {
                var hasPermission = _userPermissionService.CheckPermissionForUserAsync(context.User, requirement.Permission);
                if (hasPermission.Result)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}

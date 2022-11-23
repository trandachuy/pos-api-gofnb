using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GoFoodBeverage.POS.Application.Features.Settings.Queries;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class PermissionController : BaseApiController
    {
        public PermissionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-permissions")]
        public async Task<IActionResult> GetPermissionsAsync()
        {
            var response = await _mediator.Send(new GetPermissionsRequest());
            return await SafeOkAsync(response);
        }
    }
}

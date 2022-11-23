using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Package.Queries;
using GoFoodBeverage.Application.Features.Package.Commands;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/order-packages")]
    public class OrderPackageController : BaseApiController
    {
        public OrderPackageController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("list")]
        [HasPermission(EnumPermission.INTERNAL_TOOL)]
        public async Task<IActionResult> GetOrderPackagesAsync([FromQuery] GetOrderPackagesRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPut]
        [Route("action")]
        [HasPermission(EnumPermission.INTERNAL_TOOL)]
        public async Task<IActionResult> GetOrderPackagesAsync([FromBody] OrderPackageActionRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
    }
}

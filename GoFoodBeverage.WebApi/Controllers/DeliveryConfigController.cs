using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.DeliveryConfigs.Commands;
using GoFoodBeverage.Application.Features.DeliveryMethods.Queries;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class DeliveryConfigController : BaseApiController
    {
        public DeliveryConfigController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPut]
        [Route("update-delivery-config")]
        public async Task<IActionResult> UpdateDeliveryConfigAsync([FromBody] UpdateDeliveryConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-ahamove-config")]
        public async Task<IActionResult> UpdateAhaMoveConfigAsync([FromBody] UpdateAhaMoveConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

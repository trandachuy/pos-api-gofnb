using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.OrderSessions.Commands;
using GoFoodBeverage.POS.Application.Features.OrderSessions.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class KitchenController : BaseApiController
    {
        public KitchenController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-kitchen-order-sessions")]
        public async Task<IActionResult> GetKitchenOrderSessionsAsync([FromQuery] GetKitchenOrderSessionsInStoreBranchRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-order-session-status")]
        public async Task<IActionResult> UpdateOrderSessionStatusAsync([FromBody] UpdateOrderSessionStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-order-item-status")]
        public async Task<IActionResult> UpdateOrderItemStatusAsync([FromBody] UpdateOrderItemStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

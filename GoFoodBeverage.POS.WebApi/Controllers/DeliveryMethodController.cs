using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.DeliveryMethods.Commands;
using GoFoodBeverage.POS.Application.Features.DeliveryMethods.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class DeliveryMethodController : BaseApiController
    {
        public DeliveryMethodController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-delivery-method")]
        public async Task<IActionResult> GetDeliveryMethodsAsync([FromQuery] GetDeliveryMethodsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("estimate-ahamove-shipping-fee")]
        public async Task<IActionResult> EstimateShippingFeeAhamoveRequestAsync([FromBody] EstimateShippingFeeAhamoveRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("calculate-store-shipping-fee")]
        public async Task<IActionResult> CalculateStoreShippingFeeRequestAsync([FromBody] CalculateStoreShippingFeeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

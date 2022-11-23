using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.DeliveryMethods.Queries;
using GoFoodBeverage.Application.Features.DeliveryMethods.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class DeliveryMethodController : BaseApiController
    {
        public DeliveryMethodController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-all-delivery-method")]
        public async Task<IActionResult> GetDeliveryMethodsAsync([FromQuery]GetDeliveryMethodsRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("update-status-delivery-method-by-id")]
        public async Task<IActionResult> UpdateStatusDeliveryMethodByIdAsync([FromBody] UpdateStatusDeliveryMethodByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpGet]
        [AllowAnonymous]
        [Route("get-delivery-methods-by-store-id")]
        public async Task<IActionResult> GetDeliveryMethodsByStoreIdAsync([FromQuery] GetDeliveryMethodsByStoreIdRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("get-estimate-fee-delivery-methods-by-address")]
        public async Task<IActionResult> GetEstimateFeeDeliveryMethodsByAddressAsync([FromBody] GetEstimateFeeDeliveryMethodsByAddressRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }

        #endregion
    }
}

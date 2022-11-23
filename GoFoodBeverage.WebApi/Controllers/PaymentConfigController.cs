using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Payments.Queries;
using GoFoodBeverage.Application.Features.Payments.Commands;
using Microsoft.AspNetCore.Authorization;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class PaymentConfigController : BaseApiController
    {
        public PaymentConfigController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-payment-configs")]
        public async Task<IActionResult> GetAllStorePaymentConfigAsync([FromQuery] GetAllStorePaymentConfigRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-payment-config")]
        public async Task<IActionResult> UpdatePaymentConfigAsync([FromBody] UpdatePaymentConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("enable-payment-config")]
        public async Task<IActionResult> EnablePaymentConfigAsync([FromBody] EnablePaymentConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-payment-configuration-by-store-id")]
        public async Task<IActionResult> GetPaymentConfigurationByStoreIdAsync([FromQuery] GetPaymentConfigurationByStoreIdRequest query)
        {
            var response = await _mediator.Send(query);
            return await SafeOkAsync(response);
        }
    }
}

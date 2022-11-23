using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Customer.Commands;
using GoFoodBeverage.POS.Application.Features.Customer.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class CustomerController : BaseApiController
    {
        public CustomerController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-pos-customers")]
        public async Task<IActionResult> GetPosCustomersAsync([FromQuery] GetPosCustomersRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-by-id")]
        public async Task<IActionResult> GetCustomerByIdAsync([FromQuery] GetCustomerByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-customer")]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-customer")]
        public async Task<IActionResult> UpdateCustomerAsync([FromBody] UpdateCustomerRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

using GoFoodBeverage.Application.Features.Customer.Commands;
using GoFoodBeverage.Application.Features.Customer.Queries;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class CustomerSegmentController : BaseApiController
    {
        public CustomerSegmentController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-customer-segments")]
        public async Task<IActionResult> GetCustomerSegmentsAsync([FromQuery] GetCustomerSegmentsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-segment-by-id/{id}")]
        public async Task<IActionResult> GetCustomerSegmentByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetCustomerSegmentByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-customer-segment")]
        public async Task<IActionResult> CreateCustomerSegmentAsync([FromBody] CreateCustomerSegmentRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-customer-segment")]
        public async Task<IActionResult> UpdateCustomerSegmentAsync([FromBody] UpdateCustomerSegmentRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-customer-segment-by-id/{id}")]
        public async Task<IActionResult> DeleteCustomerSegmentByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteCustomerSegmentByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-customer-segment-by-store-id/{storeId}")]
        public async Task<IActionResult> GetCustomerSegmentByStoreId(Guid? storeId)
        {
            var response = await _mediator.Send(new GetCustomerSegmentInCurrentStoreRequest() { StoreId = storeId});
            return await SafeOkAsync(response);
        }
    }
}

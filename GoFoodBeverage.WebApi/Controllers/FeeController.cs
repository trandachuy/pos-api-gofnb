using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Fees.Queries;
using GoFoodBeverage.Application.Features.Fees.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class FeeController : BaseApiController
    {
        public FeeController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-fees")]
        [HasPermission(EnumPermission.VIEW_FEE)]
        public async Task<IActionResult> GetFeesAsync([FromQuery] GetAllFeeInStoreRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-fee-detail-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_FEE)]
        public async Task<IActionResult> GetFeeDetailByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetFeeDetailByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-fee-management")]
        [HasPermission(EnumPermission.CREATE_FEE)]
        public async Task<IActionResult> CreateFeeManagementAsync([FromBody] CreateFeeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("stop-fee-by-id/{id}")]
        [HasPermission(EnumPermission.STOP_FEE)]
        public async Task<IActionResult> StopFeeByIdAsync([FromRoute] StopFeeByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-fee-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_FEE)]
        public async Task<IActionResult> DeleteFeeByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteFeeByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}

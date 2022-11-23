using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Stores.Queries;
using GoFoodBeverage.Application.Features.Stores.Commands;
using GoFoodBeverage.Interfaces;
using System;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class BranchController : BaseApiController
    {
        public BranchController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-branch-managements")]
        public async Task<IActionResult> GetBranchManagementsAsync([FromQuery] GetBranchManagementRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
       
        [HttpGet]
        [Route("get-all-branch-management")]
        public async Task<IActionResult> GetAllBranchsAsync()
        {
            var response = await _mediator.Send(new GetAllBranchRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-branch-by-id/{id}")]
        public async Task<IActionResult> GetBranchByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetBranchByIdRequest() { BranchId = id});
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-branch-management")]
        public async Task<IActionResult> CreateBranchManagementAsync([FromBody] CreateBranchRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("update-branch-by-id")]
        public async Task<IActionResult> UpdateBranchByIdAsync([FromBody] UpdateBranchByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-branch-by-id/{branchId}")]
        [HasPermission(EnumPermission.ADMIN)]
        public async Task<IActionResult> DeleteStoreBranchByIdAsync([FromRoute] DeleteStoreBranchByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Areas.Queries;
using GoFoodBeverage.Application.Features.Areas.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class AreaController : BaseApiController
    {
        public AreaController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-area-management-by-branch-id")]
        [HasPermission(EnumPermission.VIEW_AREA_TABLE)]
        public async Task<IActionResult> GetAreaManagementByBranchId([FromQuery] GetAreasByBranchIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-areas-by-branch-id")]
        [HasPermission(EnumPermission.VIEW_AREA_TABLE)]
        public async Task<IActionResult> GetAreasByBranchIdAsync([FromQuery] GetActiveAreasByBranchIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-area-by-id")]
        [HasPermission(EnumPermission.VIEW_AREA_TABLE)]
        public async Task<IActionResult> GetAreaByIdAsync([FromQuery] GetAreaByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-area-management")]
        [HasPermission(EnumPermission.CREATE_AREA_TABLE)]
        public async Task<IActionResult> CreateAreaManagementAsync([FromBody] CreateAreaRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-area")]
        [HasPermission(EnumPermission.EDIT_AREA_TABLE)]
        public async Task<IActionResult> UpdateAreaAsync([FromBody] UpdateAreaRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-area-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_AREA_TABLE)]
        public async Task<IActionResult> DeleteAreaByIdAsync([FromRoute] DeleteAreaByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

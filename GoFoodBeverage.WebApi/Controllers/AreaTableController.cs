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
    public class AreaTableController : BaseApiController
    {
        public AreaTableController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-area-tables-by-branch")]
        [HasPermission(EnumPermission.VIEW_AREA_TABLE)]
        public async Task<IActionResult> GetListAreaTablesByBranchIdAsync([FromQuery] GetListAreaTableByBranchIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-area-table-by-id")]
        [HasPermission(EnumPermission.VIEW_AREA_TABLE)]
        public async Task<IActionResult> GetAreaTableByIdAsync([FromQuery] GetAreaTableByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-area-table-by-area-id")]
        [HasPermission(EnumPermission.CREATE_AREA_TABLE)]
        public async Task<IActionResult> CreateAreaTableByAreaIdAsync([FromBody] CreateAreaTableRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-area-table-by-area-id")]
        [HasPermission(EnumPermission.EDIT_AREA_TABLE)]
        public async Task<IActionResult> UpdateProductCategoryAsync([FromBody] UpdateAreaTableRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-area-table-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_AREA_TABLE)]
        public async Task<IActionResult> DeleteAreaTableByIdAsync([FromRoute] DeleteAreaTableByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

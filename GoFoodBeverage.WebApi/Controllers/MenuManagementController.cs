using GoFoodBeverage.Application.Features.OnlineStoreMenus.Commands;
using GoFoodBeverage.Application.Features.OnlineStoreMenus.Queries;
using GoFoodBeverage.Application.Features.QRCodes.Queries;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Domain.Enums;
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
    public class MenuManagementController : BaseApiController
    {
        public MenuManagementController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }
        [HttpGet]
        [Route("get-create-menu-prepare-data")]
        [HasPermission(EnumPermission.CREATE_MENU_MANAGEMENT)]
        public async Task<IActionResult> GetCreateMenuPrepareDataAsync([FromQuery] GetCreateMenuPrepareDataRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-menu")]
        [HasPermission(EnumPermission.CREATE_MENU_MANAGEMENT)]
        public async Task<IActionResult> CreateMenuAsync([FromBody] CreateMenuRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-menu-management")]
        [HasPermission(EnumPermission.VIEW_MENU_MANAGEMENT)]
        public async Task<IActionResult> GetAllMenuAsync([FromQuery] GetAllMenuRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-menu-item-reference-to-page-by-page-id/{pageId}")]
        [HasPermission(EnumPermission.VIEW_PAGE)]
        public async Task<IActionResult> GetMenuItemReferenceToPageByPageIdAsync(Guid pageId)
        {
            var response = await _mediator.Send(new GetMenuItemReferenceToPageByPageIdRequest() { PageId = pageId });
            return await SafeOkAsync(response);
        }
    }
}
using GoFoodBeverage.Application.Features.Pages.Commands;
using GoFoodBeverage.Application.Features.Pages.Queries;
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
    public class PageController : BaseApiController
    {
        public PageController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService) { }

        [HttpGet]
        [Route("get-all-page")]
        [HasPermission(EnumPermission.VIEW_PAGE)]
        public async Task<IActionResult> GetAllPageAsync([FromQuery]GetAllPageRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-page-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_PAGE)]
        public async Task<IActionResult> GetPageByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetPageByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-page")]
        [HasPermission(EnumPermission.CREATE_PAGE)]
        public async Task<IActionResult> CreatePageAsync(CreatePageRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-page")]
        [HasPermission(EnumPermission.EDIT_PAGE)]
        public async Task<IActionResult> UpdatePageAsync([FromBody] UpdatePageRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-page/{pageId}")]
        [HasPermission(EnumPermission.DELETE_PAGE)]
        public async Task<IActionResult> DeletePageAsync(Guid pageId)
        {
            var response = await _mediator.Send(new DeletePageRequest() { PageId = pageId });
            return await SafeOkAsync(response);
        }
    }
}
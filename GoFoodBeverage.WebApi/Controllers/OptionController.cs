using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Options.Queries;
using GoFoodBeverage.Application.Features.Options.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class OptionController : BaseApiController
    {
        public OptionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-option-management")]
        [HasPermission(EnumPermission.VIEW_OPTION)]
        public async Task<IActionResult> GetAllOptionsAsync()
        {
            var response = await _mediator.Send(new GetAllOptionRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-option-managements")]
        [HasPermission(EnumPermission.VIEW_OPTION)]
        public async Task<IActionResult> GetOptionsAsync([FromQuery] GetOptionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-option-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_OPTION)]
        public async Task<IActionResult> GetOptionByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetOptionByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-option-management")]
        [HasPermission(EnumPermission.CREATE_OPTION)]
        public async Task<IActionResult> CreateOptionManagementAsync([FromBody] CreateOptionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-option")]
        [HasPermission(EnumPermission.EDIT_OPTION)]
        public async Task<IActionResult> UpdateOptionAsync([FromBody] UpdateOptionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-option-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_OPTION)]
        public async Task<IActionResult> DeleteOptionByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteOptionByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}

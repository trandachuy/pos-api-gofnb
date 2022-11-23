using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Settings.Queries;
using GoFoodBeverage.Application.Features.Settings.Commands;
using System;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class PermissionController : BaseApiController
    {
        public PermissionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-group-permissions")]
        public async Task<IActionResult> GetGroupPermissionsAsync([FromQuery] GetGroupPermissionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-permission-groups")]
        public async Task<IActionResult> GetPermissionGroupsAsync()
        {
            var response = await _mediator.Send(new GetPermissionGroupsRequest());
            return await SafeOkAsync(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("get-permissions")]
        public async Task<IActionResult> GetPermissionsAsync([FromQuery] string token)
        {
            var response = await _mediator.Send(new GetPermissionsRequest() { Token = token });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-group-permission-by-id/{id}")]
        public async Task<IActionResult> GetGroupPermissionByIdAsync(Guid id)
        {

            var response = await _mediator.Send(new GetGroupPermissionByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-group-permissions-management")]
        public async Task<IActionResult> GetGroupPermissionManagementAsync()
        {
            var response = await _mediator.Send(new GetGroupPermissionManagementRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-group-permission")]
        public async Task<IActionResult> CreateGroupPermissionAsync([FromBody] CreateGroupPermissionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-group-permission-by-id")]
        public async Task<IActionResult> UpdateGroupPermissionByIdAsync([FromBody] UpdateGroupPermissionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

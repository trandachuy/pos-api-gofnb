using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Customer.Queries;
using GoFoodBeverage.Application.Features.Customer.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class MembershipController : BaseApiController
    {
        public MembershipController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-memberships")]
        [HasPermission(EnumPermission.VIEW_MEMBERSHIP_LEVEL)]
        public async Task<IActionResult> GetMembershipsAsync([FromQuery] GetCustomersMembershipRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-membership-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_MEMBERSHIP_LEVEL)]
        public async Task<IActionResult> GetMembershipByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetCustomerMembershipByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-membership")]
        [HasPermission(EnumPermission.CREATE_MEMBERSHIP_LEVEL)]
        public async Task<IActionResult> CreateMembershipAsync([FromBody] CreateCustomerMembershipRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-membership")]
        [HasPermission(EnumPermission.EDIT_MEMBERSHIP_LEVEL)]
        public async Task<IActionResult> UpdateMembershipAsync([FromBody] UpdateCustomerMembershipRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-membership-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_MEMBERSHIP_LEVEL)]
        public async Task<IActionResult> DeleteOptionByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteCustomerMembershipByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }
    }
}

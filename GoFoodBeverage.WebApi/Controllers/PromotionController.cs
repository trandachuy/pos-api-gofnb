using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Promotions.Queries;
using GoFoodBeverage.Application.Features.Promotions.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class PromotionController : BaseApiController
    {
        public PromotionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-promotions")]
        [HasPermission(EnumPermission.VIEW_PROMOTION)]
        public async Task<IActionResult> GetPromotionsAsync([FromQuery] GetPromotionsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-promotion-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_PROMOTION)]
        public async Task<IActionResult> GetPromotionByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetPromotionByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-promotion")]
        [HasPermission(EnumPermission.CREATE_PROMOTION)]
        public async Task<IActionResult> CreatePromotionAsync([FromBody] CreatePromotionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("stop-promotion-by-id/{id}")]
        [HasPermission(EnumPermission.STOP_PROMOTION)]
        public async Task<IActionResult> StopPromotionByIdAsync([FromRoute] StopPromotionByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-promotion")]
        [HasPermission(EnumPermission.EDIT_PROMOTION)]
        public async Task<IActionResult> UpdatePromotionAsync([FromBody] UpdatePromotionRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-promotion-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_PROMOTION)]
        public async Task<IActionResult> DeletePromotionByIdAsync([FromRoute] DeletePromotionByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpGet]
        [AllowAnonymous]
        [Route("get-promotions-in-branch")]
        public async Task<IActionResult> GetPromotionsInBranchAsync([FromQuery] GetPromotionsInStoreRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-promotion-detail-by-id")]
        public async Task<IActionResult> GetPromotionDetailByIdAsync([FromQuery] GetPromotionDetailByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #endregion
    }
}

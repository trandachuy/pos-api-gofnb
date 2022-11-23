using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Products.Queries;
using GoFoodBeverage.Application.Features.Products.Commands;
using GoFoodBeverage.Application.Features.Promotions.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class ComboController : BaseApiController
    {
        public ComboController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-prepare-create-product-combo-data")]
        public async Task<IActionResult> GetPrepareCreateProductComboDataAsync()
        {
            var response = await _mediator.Send(new GetPrepareCreateProductComboDataRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-combos")]
        [HasPermission(EnumPermission.VIEW_COMBO)]
        public async Task<IActionResult> GetCombosAsync([FromQuery] GetCombosRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-combo-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_COMBO)]
        public async Task<IActionResult> GetComboByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetComboByIdRequest() { Id = id });
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-combo")]
        [HasPermission(EnumPermission.CREATE_COMBO)]
        public async Task<IActionResult> CreateComboAsync([FromBody] CreateComboRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-combo")]
        [HasPermission(EnumPermission.EDIT_COMBO)]
        public async Task<IActionResult> EditComBoAsync([FromBody] UpdateComboRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-combo-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_COMBO)]
        public async Task<IActionResult> DeleteComboByIdAsync([FromRoute] DeleteComboByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("stop-combo-by-id/{id}")]
        [HasPermission(EnumPermission.STOP_COMBO)]
        public async Task<IActionResult> StopComboByIdAsync([FromRoute] StopComboByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Tax.Queries;
using GoFoodBeverage.Application.Features.Tax.Commands;
using GoFoodBeverage.Application.Features.Taxes.Queries;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class TaxController : BaseApiController
    {
        public TaxController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-all-tax")]
        [HasPermission(EnumPermission.VIEW_TAX)]
        public async Task<IActionResult> GetAllTaxAsync([FromQuery] GetAllTaxRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-tax-by-tax-type/{id}")]
        [HasPermission(EnumPermission.VIEW_TAX)]
        public async Task<IActionResult> GetAllTaxTypeAsync([FromQuery] GetAllTaxesByTaxTypeRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-tax-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_TAX)]
        public async Task<IActionResult> GetTaxByIdAsync([FromRoute] GetTaxByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-tax")]
        [HasPermission(EnumPermission.CREATE_TAX)]
        public async Task<IActionResult> CreateTaxAsync([FromBody] CreateTaxRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpDelete]
        [Route("delete-tax-by-id/{id}")]
        [HasPermission(EnumPermission.DELETE_TAX)]
        public async Task<IActionResult> DeleteTaxByIdAsync([FromRoute] DeleteTaxByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

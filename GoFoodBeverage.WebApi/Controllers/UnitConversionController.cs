using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Units.Commands;
using GoFoodBeverage.Application.Features.Units.Queries;
using GoFoodBeverage.Interfaces;
using System;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class UnitConversionController : BaseApiController
    {
        public UnitConversionController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-unit-conversion-by-unit-id")]
        public async Task<IActionResult> GetUnitConversionByUnitIdAsync([FromQuery] GetUnitConversionByUnitIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-unit-conversions")]
        public async Task<IActionResult> GetUnitConversionAsync()
        {
            var response = await _mediator.Send(new GetUnitConversionsRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-unit-conversions")]
        public async Task<IActionResult> CreateUnitConversionsAsync([FromBody] CreateUnitConversionsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-unit-conversions")]
        public async Task<IActionResult> UpdateUnitConversionsAsync([FromBody] UpdateUnitConversionsRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-unit-conversions-by-material-id/{materialId}")]
        public async Task<IActionResult> GetUnitConversionsByMaterialIdAsync(Guid materialId)
        {
            var response = await _mediator.Send(new GetUnitConversionsByMaterialIdRequest() { MaterialId = materialId});
            return await SafeOkAsync(response);
        }
    }
}

using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Units.Queries;
using GoFoodBeverage.Application.Features.Units.Commands;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class UnitController : BaseApiController
    {
        public UnitController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-units")]
        public async Task<IActionResult> GetUnitsAsync()
        {
            var response = await _mediator.Send(new GetUnitsRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-unit")]
        public async Task<IActionResult> CreateUnitAsync([FromBody] CreateUnitRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

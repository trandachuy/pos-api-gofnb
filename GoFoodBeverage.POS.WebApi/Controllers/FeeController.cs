using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Fees.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class FeeController : BaseApiController
    {
        public FeeController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-fees-active")]
        public async Task<IActionResult> GetFeesActiveAsync([FromQuery] GetFeesRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Areas.Queries;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class AreaController : BaseApiController
    {
        public AreaController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }


        [HttpGet]
        [Route("get-all-areas-using")]
        public async Task<IActionResult> GetAllAreasInUseAsync([FromQuery] GetAllAreasInUseRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-all-areas-activated")]
        public async Task<IActionResult> GetAllAreasActivatedAsync([FromQuery] GetAllAreasActivatedRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

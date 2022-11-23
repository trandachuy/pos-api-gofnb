using GoFoodBeverage.Application.Features.Stamps.Commands;
using GoFoodBeverage.Application.Features.Stamps.Queries;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class StampController : BaseApiController
    {
        public StampController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-stamp-config-by-store-id")]
        public async Task<IActionResult> GetStampConfigByStoreIdAsync([FromRoute] GetStampConfigByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-stamp-config")]
        public async Task<IActionResult> UpdateStampConfigAsync([FromBody] UpdateStampConfigRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

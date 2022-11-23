using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Slideshow.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class SlideshowController : BaseApiController
    {
        public SlideshowController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-slideshow")]
        public async Task<IActionResult> GetSlider([FromQuery] GetSlideshowInfoRequest request)
        {
            var response = await _mediator.Send(request);

            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-store-banners/{type}")]
        public async Task<IActionResult> GetFullScreenBannersAsync([FromRoute] GetStoreBannersAsyncRequest request)
        {
            var response = await _mediator.Send(request);

            return await SafeOkAsync(response);
        }
    }
}

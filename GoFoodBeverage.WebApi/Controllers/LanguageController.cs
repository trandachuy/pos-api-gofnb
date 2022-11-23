using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Languages.Queries;
using GoFoodBeverage.Application.Features.Languages.Commands;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class LanguageController : BaseApiController
    {
        public LanguageController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-list-language-not-contain-store-id")]
        public async Task<IActionResult> GetListLanguageNotContainStoreIdAsync()
        {
            var response = await _mediator.Send(new GetListLanguageNotContainStoreIdRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-list-language-store-by-store-id")]
        public async Task<IActionResult> GetListLanguageStoreByStoreId()
        {
            var response = await _mediator.Send(new GetListLanguageStoreByStoreIdRequest());
            return SafeOk(response);
        }

        [HttpGet]
        [Route("get-list-language-by-store-id-and-is-publish")]
        public async Task<IActionResult> GetListLanguageByStoreIdAndIsPublish()
        {
            var response = await _mediator.Send(new GetListLanguageByStoreIdAndIsPublishRequest());
            return SafeOk(response);
        }

        [HttpPost]
        [Route("create-language-store")]
        public async Task<IActionResult> CreateLanguageStore([FromBody] CreateLanguageStoreRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-is-publish-by-id")]
        public async Task<IActionResult> UpdateIsPublishById([FromBody] UpdateIsPublishByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
    }
}

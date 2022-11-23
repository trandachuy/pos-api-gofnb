using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Stores.Queries;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [Authorize]
    public class BranchController : BaseApiController
    {
        public BranchController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-branches-by-store-id/{storeId}")]
        [Obsolete("remove this function when next version of app")]
        public async Task<IActionResult> GetBranchesByStoreIdAsync([FromRoute] GetBranchesByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-branches-by-account-id")]
        public async Task<IActionResult> GetBranchesByAccountIdAsync([FromQuery] GetBranchesByStoreIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

    }
}

using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Users.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class LoginController : BaseApiController
    {
        public LoginController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Authenticate.Request request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("refresh-token-and-permissions")]
        public async Task<IActionResult> RefreshTokenAndPermissionsAsync([FromHeader] RefreshTokenAndPermissionsRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        /// <summary>
        /// This method is used to allow customers to access our system when the customer use the mobile app.
        /// </summary>
        /// <param name="request">The HTTP data.</param>
        /// <returns>The JSON object.</returns>
        [HttpPost]
        [Route("customer")]
        public async Task<IActionResult> Customer([FromBody] CustomerAuthenticationRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("internal-tool")]
        public async Task<IActionResult> InteralToolAuthenticate([FromBody] InternalToolAuthenticate.Request request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        /// <summary>
        /// This is a method that checks a user's credentials and returns a list of stores owned by that user
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The JSON store list object</returns>
        [HttpPost]
        [Route("check-before-authenticate")]
        public async Task<IActionResult> CheckBeforeAuthenticate([FromBody] CheckBeforeAuthenticateRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

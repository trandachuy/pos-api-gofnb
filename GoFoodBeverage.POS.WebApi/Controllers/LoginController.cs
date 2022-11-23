using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Users.Commands;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class LoginController : BaseApiController
    {
        public LoginController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] PosAuthenticateRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
    }
}

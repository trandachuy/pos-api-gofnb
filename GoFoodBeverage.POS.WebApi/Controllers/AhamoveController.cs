using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.DeliveryMethods.Commands;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    [AllowAnonymous]
    public class AhamoveController : BaseApiController
    {
        public AhamoveController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpPost]
        [Route("update-order-status")]
        public async Task<IActionResult> UpdateAhamoveOrderStatusRequestAsync([FromBody] UpdateAhamoveOrderStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
    }
}

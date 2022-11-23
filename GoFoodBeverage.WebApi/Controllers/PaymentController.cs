using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Application.Features.Payments.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    public class PaymentController : BaseApiController
    {
        public PaymentController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        #region MOMO Services

        [HttpPost]
        [Route("create-mobile-momo-payment")]
        public async Task<IActionResult> CreateMobileMoMoPaymentAsync([FromBody] CreateMobileMoMoPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        #endregion

    }
}

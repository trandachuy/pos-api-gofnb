using GoFoodBeverage.Application.Features.Payments.Commands;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Features.Payments.Commands;
using GoFoodBeverage.POS.Application.Features.Payments.Queries;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class PaymentController : BaseApiController
    {
        public PaymentController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        #region MOMO Services

        [HttpPost]
        [Route("create-normal-payment")]
        public async Task<IActionResult> CreateNormalPaymentAsync([FromBody] CreateNormalPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("create-pos-payment")]
        public async Task<IActionResult> CreatePosPaymentAsync([FromBody] CreateMomoPaymentByCustomerCodeRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("refund-by-order-id")]
        public async Task<IActionResult> CreateRefundAsync([FromBody] CreatePaymentRefundRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpGet]
        [Route("get-order-status")]
        public async Task<IActionResult> GetOrderStatusAsync([FromQuery] GetMoMoOrderStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        #endregion

        #region VNPAY Services

        [HttpPost]
        [Route("create-vnpay-payment")]
        public async Task<IActionResult> CreateVNPayPaymentRequestAsync([FromBody] CreateVNPayPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpGet]
        [Route("get-vnpay-payment-status")]
        public async Task<IActionResult> GetVNPayPaymentStatusAsync([FromQuery] GetVNPayPaymentStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        /// <summary>
        /// This method is used to update order status and payment transaction when the order has been completed.
        /// </summary>
        /// <param name="request">The HTTP data.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("vnpay-update-order-by-qr-code")]
        public async Task<IActionResult> VnPayUpdateOrderByQrCode([FromBody] VnPayUpdateOrderByQrCodeRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        /// <summary>
        /// This method is used to update order status and payment transaction when the order has been completed (for Mobile app).
        /// </summary>
        /// <param name="request">The HTTP data.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("vnpay-update-order-by-sdk")]
        public async Task<IActionResult> VnPayUpdateOrderBySdk([FromQuery] VnPayUpdateOrderBySdkRequest request)
        {
            var response = await _mediator.Send(request);
            return Redirect(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpn([FromQuery] VnPayIpnRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
        #endregion
    }
}

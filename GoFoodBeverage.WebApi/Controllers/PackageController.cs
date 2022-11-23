using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Package.Queries;
using GoFoodBeverage.Application.Features.Package.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class PackageController : BaseApiController
    {
        public PackageController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-packages-pricing")]
        public async Task<IActionResult> GetPackagesPricingAsync()
        {
            var response = await _mediator.Send(new GetPackagePricingsRequest());
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-list-package-order")]
        public async Task<IActionResult> GetListPackageOrderAsync()
        {
            var response = await _mediator.Send(new GetListPackageOrderRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-bank-transfer-payment")]
        public async Task<IActionResult> CreateBankTransferPaymentAsync([FromBody] CreateBankTransferPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-vnpay-payment")]
        public async Task<IActionResult> CreateVNPayPaymentRequestAsync([FromBody] CreateVNPayPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("update-vnpay-payment")]
        public async Task<IActionResult> UpdateVNPayPaymentRequestAsync([FromBody] UpdateVNPayPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
    }
}

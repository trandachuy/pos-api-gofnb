using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.WebApi.Controllers.Base;
using GoFoodBeverage.POS.Application.Features.Orders.Queries;
using GoFoodBeverage.POS.Application.Features.Orders.Commands;
using Microsoft.AspNetCore.Authorization;

namespace GoFoodBeverage.POS.WebApi.Controllers
{
    public class OrderController : BaseApiController
    {
        public OrderController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-pos-order-by-id-for-payment/{id}")]
        public async Task<IActionResult> GetPOSOrderByIdForPaymentAsync([FromRoute] GetPOSOrderByIdForPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-detail-by-id")]
        public async Task<IActionResult> GetOrderDetailByIdAsync([FromHeader] GetOrderDetailByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-prepare-order-edit-data/{orderId}")]
        public async Task<IActionResult> GetPrepareOrderEditDataRequestAsync([FromRoute] Guid orderId)
        {
            var response = await _mediator.Send(new GetPrepareOrderEditDataRequest() { OrderId = orderId});
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-order-detail-to-print/{orderId}")]
        public async Task<IActionResult> GetDetailOrderToPrint([FromRoute] Guid orderId)
        {
            var response = await _mediator.Send(new GetDetailOrderToPrintRequest() { OrderId = orderId });
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-default-bill-configuration")]
        public async Task<IActionResult> GetDefaultBillConfigurationAsync()
        {
            var response = await _mediator.Send(new GetDefaultBillConfigurationRequest());
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("print-order-stamp-data")]
        public async Task<IActionResult> PrintOrderStampDataAsync([FromBody] GetOrderStampDataRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("get-all-pos-order-by-branch")]
        public async Task<IActionResult> GetOrderManagementAsync([FromBody] GetAllPosOrderByBranchRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-pos-order")]
        public async Task<IActionResult> CreatePOSOrderAsync([FromBody] CreateOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("pay-order")]
        public async Task<IActionResult> PayOrderAsync([FromBody] PayOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatusAsync([FromBody] UpdateOrderStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpGet]
        [Route("check-prepared-status-order-item/{orderId}/{productId?}")]
        public async Task<IActionResult> GetOrderItemStatusById([FromRoute] CheckPreparedStatusForOrderItemRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }


        [HttpGet]
        [Route("check-order-item-status-from-kitchen-by-order-id/{orderId}")]
        public async Task<IActionResult> CheckOrderItemStatusFromKitchenByOrderIdAsync([FromRoute] CheckOrderItemStatusFromKitchenRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPost]
        [Route("check-add-product-for-order")]
        public async Task<IActionResult> CheckAddProductForOrder([FromBody] CheckAddProductForOrderRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }
    }
}

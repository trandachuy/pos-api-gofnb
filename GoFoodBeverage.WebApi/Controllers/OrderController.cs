using System;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.WebApi.Controllers.Base;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Application.Features.Orders.Queries;
using GoFoodBeverage.Application.Features.Orders.Commands;

namespace GoFoodBeverage.WebApi.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        public OrderController(IMediator mediator, IUserActivityService userActivityService) : base(mediator, userActivityService)
        {
        }

        [HttpGet]
        [Route("get-order-management")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderManagementAsync([FromQuery] GetOrdersManagementRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-report-by-filter")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderReportByFilterAsync([FromQuery] GetOrderReportByFilterRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-by-id/{id}")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderByIdAsync([FromHeader] GetOrderByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-business-summary-widget")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderBusinessSummaryWidgetAsync([FromQuery] GetOrderBusinessSummaryWidgetRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-top-selling-product")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderTopSellingProductAsync([FromQuery] GetOrderTopSellingProductRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("calculate-statistical-data")]
        public async Task<IActionResult> CalculateStatisticalDataAsync([FromBody] GetStatisticalDataRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-business-revenue-widget")]
        public async Task<IActionResult> GetOrderBusinessRevenueWidgetAsync([FromQuery] GetOrderBusinessRevenueWidgetRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-product-report")]
        public async Task<IActionResult> GetOrderProductReportAsync([FromQuery] GetOrderProductReportRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-revenue-by-type")]
        public async Task<IActionResult> GetRevenueByTypeAsync([FromQuery] GetOrderRevenueSummaryReportRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        #region GoFood App

        [HttpGet]
        [Route("get-order-detail-by-id")]
        public async Task<IActionResult> GetOrderDetailByIdAsync([FromQuery] GetOrderDetailByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }

        [HttpPost]
        [Route("create-order")]
        public async Task<IActionResult> CreateOrderAsync(CreateOrderRequest command)
        {
            var response = await _mediator.Send(command);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("update-status-order-payment")]
        public async Task<IActionResult> UpdateStatusOrderPaymentAsync([FromBody] UpdateStatusOrderPaymentRequest request)
        {
            var response = await _mediator.Send(request);
            return SafeOk(response);
        }

        [HttpPut]
        [Route("update-order")]
        public async Task<IActionResult> UpdateOrderAsync(UpdateOrderStatusRequest command)
        {
            var response = await _mediator.Send(command);
            return await SafeOkAsync(response);
        }

        [HttpGet]
        [Route("get-order-sold-product")]
        [HasPermission(EnumPermission.VIEW_ORDER)]
        public async Task<IActionResult> GetOrderSoldProductAsync([FromQuery] GetOrderSoldProductRequest request)
        {
            var response = await _mediator.Send(request);
            return await SafeOkAsync(response);
        }
        
        [HttpPut]
        [Route("update-order-by-vnpay-wallet-sdk")]
        public async Task<IActionResult> UpdateOrderByVnPayWalletSdkAsync(UpdateOrderFromVnPayWalletRequest command)
        {
            var response = await _mediator.Send(command);
            return await SafeOkAsync(response);
        }

        [HttpPut]
        [Route("cancel-order")]
        public async Task<IActionResult> CancelOrderAsync(CancelOrderRequest command)
        {
            var response = await _mediator.Send(command);
            return await SafeOkAsync(response);
        }
        #endregion
    }
}

using System;
using MediatR;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Payment.VNPay.Enums;
using GoFoodBeverage.Payment.VNPay.Model;

namespace GoFoodBeverage.POS.Application.Features.Payments.Commands
{
    public class VnPayUpdateOrderBySdkRequest : IRequest<string>
    {

        /// <summary>
        /// The order code in the Order table.
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// This is the date and time received at the step Create QA Code in the Payment Method modal on the POS Dashboard page.
        /// </summary>
        public string VnPayCreateDate { get; set; }

        [FromQuery(Name = "vnp_Amount")]
        public string Amount { get; set; }

        [FromQuery(Name = "vnp_BankCode")]
        public string BankCode { get; set; }

        [FromQuery(Name = "vnp_OrderInfo")]
        public string OrderInfo { get; set; }

        [FromQuery(Name = "vnp_PayDate")]
        public string PayDate { get; set; }

        [FromQuery(Name = "vnp_ResponseCode")]
        public string ResponseCode { get; set; }

        [FromQuery(Name = "vnp_SecureHash")]
        public string SecureHash { get; set; }

        [FromQuery(Name = "vnp_TmnCode")]
        public string TerminalId { get; set; }

        [FromQuery(Name = "vnp_TransactionNo")]
        public string TransactionNo { get; set; }

        [FromQuery(Name = "vnp_TransactionStatus")]
        public string TransactionStatus { get; set; }

        [FromQuery(Name = "vnp_TxnRef")]
        public string TxnRef { get; set; }

        public string UrlForDebugging { get; set; }
    }

    public class VnPayUpdateOrderBySdkResponse
    {
        public bool IsSuccess { get; set; }

    }

    public class VnPayUpdateOrderBySdkHandler : IRequestHandler<VnPayUpdateOrderBySdkRequest, string>
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IVNPayService _vnPayService;

        private readonly IUserProvider _userProvider;

        private readonly IOrderService _orderService;

        private readonly IHttpContextAccessor _httpContext;

        public VnPayUpdateOrderBySdkHandler(
            IUnitOfWork unitOfWork,
            IVNPayService vnPayService,
            IUserProvider userProvider,
            IOrderService orderService,
            IHttpContextAccessor httpContext
        )
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _vnPayService = vnPayService;
            _userProvider = userProvider;
            _orderService = orderService;
        }



        /// <summary>
        /// This method is used to handle the current request.
        /// </summary>
        /// <param name="request">The HTTP data</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<string> Handle(VnPayUpdateOrderBySdkRequest request, CancellationToken cancellationToken)
        {
           request.UrlForDebugging = _httpContext.HttpContext.Request.QueryString.Value;

            long transactionId = 0;
            long.TryParse(request.TxnRef, out transactionId);

            // Find order transaction from database.
            var orderTransaction = transactionId == 0 ? null : await _unitOfWork.
                OrderPaymentTransactions.
                GetAll().
                FirstOrDefaultAsync(opt => opt.TransId == transactionId);

            if (orderTransaction == null)
            {
                return VnPayLinkConstants.FAIL;
            }

            // Get the current user information from request.
            // Find order in the database by the order code, for example: 70
            var order = await _unitOfWork.Orders.GetAll()
              .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
              .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
              .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
              .Include(x => x.OrderFees)
              .Include(x => x.AreaTable).ThenInclude(x => x.Area)
              .FirstOrDefaultAsync(o => o.Id == orderTransaction.OrderId);

            // Get the store's payment configuration.
            var paymentConfigForVnPay = await _unitOfWork.
                PaymentConfigs.
                GetPaymentConfigAsync(order.StoreId, EnumPaymentMethod.VNPay);

            // The token is used to access the payment provider VNPay.
            var config = new VNPayConfigModel();
            config.TerminalId = paymentConfigForVnPay.PartnerCode;
            config.SecretKey = paymentConfigForVnPay.SecretKey;

            // Create a new query to check the real order.
            var resultFromVnPay = await _vnPayService.QueryAsync(
                config,
                request.TxnRef,
                request.OrderInfo,
                request.VnPayCreateDate,
                request.VnPayCreateDate
            );

            // Handle data by the response code.
            if (request.ResponseCode == VNPayResponseCode.Success &&
                resultFromVnPay.ResponseCode == VNPayResponseCode.Success)
            {
                return await
                        UpdateOrderInfo(
                            request,
                            resultFromVnPay,
                            order,
                            orderTransaction,
                            EnumOrderPaymentStatus.Paid,
                            EnumOrderStatus.Processing
                        );
            }
            else
            {
                return await
                        UpdateOrderInfo(
                            request,
                            resultFromVnPay,
                            order,
                            orderTransaction,
                            EnumOrderPaymentStatus.Unpaid,
                            EnumOrderStatus.Canceled
                        );
            }

        }

        private async Task<string> UpdateOrderInfo(
            VnPayUpdateOrderBySdkRequest request,
            VNPayQueryPaymentStatusResponse resultFromVnPay,
            Order order,
            OrderPaymentTransaction orderTransaction,
            EnumOrderPaymentStatus orderPaymentStatus,
            EnumOrderStatus orderStatus
        )
        {
            // Data to response to the client.
            string urlToRedirect = VnPayLinkConstants.CANCEL;

            string oldOrder = order.ToJsonWithCamelCase();
            string newOrder = string.Empty;
            // If the order is already in the system, update the current order status.
            if (order != null && orderTransaction != null)
            {
                DateTime lastTime = DateTime.UtcNow;
                bool paymentHasBeenSuccessfully = resultFromVnPay.ResponseCode == VNPayResponseCode.Success &&
                resultFromVnPay.TransactionStatus == VNPayResponseCode.Success;

                // Update order information.
                order.PaymentMethodId = EnumPaymentMethod.CreditDebitCard;
                order.OrderPaymentStatusId = orderPaymentStatus;
                order.StatusId = orderStatus;
                order.LastSavedTime = lastTime;
                newOrder = order.ToJsonWithCamelCase();
                await _unitOfWork.Orders.UpdateAsync(order);

                // Update payment transaction information.
                orderTransaction.IsSuccess = paymentHasBeenSuccessfully;
                orderTransaction.LastSavedTime = lastTime;
                orderTransaction.ResponseData = JsonConvert.SerializeObject(resultFromVnPay);
                await _unitOfWork.OrderPaymentTransactions.UpdateAsync(orderTransaction);

                // Set data to display on the Secondary Screen and POS Dashboard page.
                if (paymentHasBeenSuccessfully)
                {
                    urlToRedirect = VnPayLinkConstants.SUCCESS;
                }
                else
                {
                    if (resultFromVnPay.ResponseCode == VnPayResponseCodeConstants.CANCEL || request.ResponseCode == VnPayResponseCodeConstants.CANCEL)
                    {
                        urlToRedirect = VnPayLinkConstants.CANCEL;
                    }
                    else
                    {
                        urlToRedirect = VnPayLinkConstants.FAIL;
                    }
                }
            }

            /// Save order history
            var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), EnumPaymentMethod.VNPay.GetName());
            var orderHistoryAddModel = new OrderHistory
            {
                OrderId = order.Id,
                OldOrrderData = oldOrder,
                NewOrderData = newOrder,
                ActionName = actionName,
                Note = $"{request.UrlForDebugging} / {JsonConvert.SerializeObject(resultFromVnPay)} / {urlToRedirect} / vn-pay-code = {resultFromVnPay.ResponseCode}",
                CreatedTime = DateTime.UtcNow
            };

            _unitOfWork.OrderHistories.Add(orderHistoryAddModel);
            await _unitOfWork.SaveChangesAsync();

            return urlToRedirect;
        }
    }
}

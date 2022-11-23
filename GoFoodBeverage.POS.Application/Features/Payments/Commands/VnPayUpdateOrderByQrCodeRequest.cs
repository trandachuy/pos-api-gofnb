using System;
using MediatR;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Payment.VNPay.Enums;
using GoFoodBeverage.Payment.VNPay.Model;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;

namespace GoFoodBeverage.POS.Application.Features.Payments.Commands
{
    public class VnPayUpdateOrderByQrCodeRequest : IRequest<VnPayUpdateOrderByQrCodeResponse>
    {
        /// <summary>
        /// The order code in the Order table.
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// This is the date and time received at the step Create QA Code in the Payment Method modal on the POS Dashboard page.
        /// </summary>
        public string VnPayCreateDate { get; set; }

        public string Amount { get; set; }

        public string BankCode { get; set; }

        public string OrderInfo { get; set; }

        public string PayDate { get; set; }

        public string ResponseCode { get; set; }

        public string SecureHash { get; set; }

        public string TerminalId { get; set; }

        public string TransactionNo { get; set; }

        public string TransactionStatus { get; set; }

        public string TxnRef { get; set; }

        public string UrlForDebugging { get; set; }
    }

    public class VnPayUpdateOrderByQrCodeResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public VnPayModel VnPayQuery { get; set; }

        public class VnPayModel
        {
            public VnPayModel(VNPayQueryPaymentStatusResponse vnpayData)
            {
                VnPayId = vnpayData.OrderId;
                ResponseCode = vnpayData.ResponseCode;
            }

            public string VnPayId { get; set; }

            public string ResponseCode { get; set; }
        }
    }

    public class RequestHandler : IRequestHandler<VnPayUpdateOrderByQrCodeRequest, VnPayUpdateOrderByQrCodeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVNPayService _vnPayService;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        private readonly IKitchenService _kitchenService;
        private readonly IMediator _mediator;

        public RequestHandler(
            IUnitOfWork unitOfWork,
            IVNPayService vnPayService,
            IUserProvider userProvider,
            IOrderService orderService,
            IKitchenService kitchenService,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
            _userProvider = userProvider;
            _orderService = orderService;
            _kitchenService = kitchenService;
            _mediator = mediator;
        }

        /// <summary>
        /// This method is used to handle the current request.
        /// </summary>
        /// <param name="request">The HTTP data</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<VnPayUpdateOrderByQrCodeResponse> Handle(VnPayUpdateOrderByQrCodeRequest request, CancellationToken cancellationToken)
        {
            // Get the current user information from request.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            string newOrder = string.Empty;
            string oldOrder = string.Empty;

            // Get the store's payment configuration.
            var paymentConfigForVnPay = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.VNPay);

            // The token is used to access the payment provider VNPay.
            var config = new VNPayConfigModel
            {
                TerminalId = paymentConfigForVnPay.PartnerCode,
                SecretKey = paymentConfigForVnPay.SecretKey
            };

            // Create a new query to check the real order.
            var resultFromVnPay = await _vnPayService.QueryAsync(
                config,
                request.TxnRef,
                request.OrderInfo,
                request.VnPayCreateDate,
                request.VnPayCreateDate
            );

            // Data to response to the client.
            var dataToResponse = new VnPayUpdateOrderByQrCodeResponse();
            var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), EnumPaymentMethod.VNPay.GetName());

            long transactionId = 0;
            long.TryParse(request.TxnRef, out transactionId);

            // Find order transaction from database.
            var orderTransaction = transactionId == 0 ? null : await _unitOfWork.
                OrderPaymentTransactions.
                GetAll().
                FirstOrDefaultAsync(opt => opt.TransId == transactionId);

            if (orderTransaction == null)
            {
                dataToResponse.IsSuccess = false;
                dataToResponse.Message = "order.orderDoesNotExistInTheSystem";
                return dataToResponse;
            }

            // Find order in the database by the order code, for example: 70
            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
              .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
              .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
              .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
              .Include(x => x.OrderFees)
              .Include(x => x.AreaTable).ThenInclude(x => x.Area)
              .FirstOrDefaultAsync(o => o.Code == request.OrderCode);

            // Handle data by the response code.
            switch (resultFromVnPay.TransactionStatus)
            {
                case VNPayResponseCode.Success:
                    oldOrder = order.ToJsonWithCamelCase();
                    // If the order is already in the system, update the current order status.
                    if (order != null && orderTransaction != null)
                    {
                        DateTime lastTime = DateTime.UtcNow;

                        // Update order information.
                        order.PaymentMethodId = EnumPaymentMethod.VNPay;

                        //Handle update order and payment status
                        bool paymentHasBeenCompleted = resultFromVnPay.ResponseCode == VNPayResponseCode.Success &&
                        resultFromVnPay.TransactionStatus == VNPayResponseCode.Success ? true : false;
                        order.OrderPaymentStatusId = paymentHasBeenCompleted ?
                            EnumOrderPaymentStatus.Paid : EnumOrderPaymentStatus.Unpaid;

                        // Get delivery method.
                        var deliveryMethod = await _unitOfWork.
                            DeliveryMethods.
                            GetAll().
                            FirstOrDefaultAsync(x => x.Id == order.DeliveryMethodId);

                        var orderStatus = await _orderService.
                            GetOrderStatusAsync(order.StoreId.Value, order.OrderTypeId, deliveryMethod?.EnumId);
                        order.StatusId = orderStatus.OrderStatus;

                        order.LastSavedTime = lastTime;
                        order.LastSavedUser = loggedUser?.AccountId;
                        newOrder = order.ToJsonWithCamelCase();
                        await _unitOfWork.Orders.UpdateAsync(order);

                        // Update payment transaction information.
                        orderTransaction.IsSuccess = paymentHasBeenCompleted;
                        orderTransaction.LastSavedTime = lastTime;
                        orderTransaction.LastSavedUser = loggedUser?.AccountId;
                        orderTransaction.ResponseData = JsonConvert.SerializeObject(resultFromVnPay);
                        await _unitOfWork.OrderPaymentTransactions.UpdateAsync(orderTransaction);

                        // Order history here.

                        // Set data to display on the Secondary Screen and POS Dashboard page.
                        dataToResponse.IsSuccess = true;
                        dataToResponse.Message = "payment.paymentSuccessful";
                    }
                    else
                    {
                        dataToResponse.Message = "order.orderDoesNotExistInTheSystem";
                    }

                    _unitOfWork.Orders.Update(order);
                    await _unitOfWork.SaveChangesAsync();

                    // Save order history

                    await _orderService.SaveOrderHistoryAsync(order.Id, oldOrder, newOrder, actionName, null, null);
                    var storeConfig = await _unitOfWork.Stores.GetStoreConfigAsync(loggedUser.StoreId.Value);
                    if (order.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid)
                    {
                        await _orderService.CalculatePoint(order, loggedUser.StoreId);
                        if (!storeConfig.IsPaymentLater)
                        {
                            await _orderService.CalMaterialQuantity(order.Id, false, false, EnumInventoryHistoryAction.CreateOrder);
                        }
                    }

                    await _mediator.Send(new CreateStaffActivitiesRequest()
                    {
                        ActionGroup = EnumActionGroup.Order,
                        ActionType = EnumActionType.PaymentStatus,
                        ObjectId = order.Id,
                        ObjectName = order.Code,
                    }, cancellationToken);

                    await _kitchenService.GetKitchenOrderSessionsAsync(cancellationToken);

                    break;

                case VNPayResponseCode.Other:
                    dataToResponse.Message = "payment.paymentUnsuccessful";
                    dataToResponse.VnPayQuery = new(resultFromVnPay);
                    break;

            }

            // Save order history
            var orderHistoryAddModel = new OrderHistory
            {
                OrderId = order.Id,
                OldOrrderData = oldOrder,
                NewOrderData = newOrder,
                ActionName = actionName,
                Note = $"{request.UrlForDebugging} / {JsonConvert.SerializeObject(resultFromVnPay)} / vn-pay-code = {resultFromVnPay.ResponseCode}",
                CreatedTime = DateTime.UtcNow
            };

            _unitOfWork.OrderHistories.Add(orderHistoryAddModel);

            return dataToResponse;
        }
    }
}

using System;
using MediatR;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Payment.VNPay;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Payment.VNPay.Enums;
using GoFoodBeverage.Payment.VNPay.Model;

namespace GoFoodBeverage.Application.Features.Orders.Commands
{
    public class UpdateOrderFromVnPayWalletRequest : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public string TxnRef { get; set; }

        public string OrderInfo { get; set; }

        public string VnPayCreateDate { get; set; }
    }

    public class UpdateOrderFromVnPayWalletRequestHandle : IRequestHandler<UpdateOrderFromVnPayWalletRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IVNPayService _vnPayService;

        public UpdateOrderFromVnPayWalletRequestHandle(IUnitOfWork unitOfWork, IVNPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
        }

        public async Task<bool> Handle(UpdateOrderFromVnPayWalletRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Get the current user information from request.
                    // Find order in the database by the order code, for example: 70
                    var order = await _unitOfWork.Orders.GetAll()
                      .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                      .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                      .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                      .Include(x => x.OrderFees)
                      .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                      .FirstOrDefaultAsync(o => o.Id == request.OrderId);

                    if (order != null)
                    {
                        if (order.OrderPaymentStatusId != EnumOrderPaymentStatus.Paid)
                        {
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

                            if (resultFromVnPay.ResponseCode == VNPayResponseCode.Success && resultFromVnPay.TransactionStatus == VNPayResponseCode.Success)
                            {
                                string oldOrder = order.ToJsonWithCamelCase();
                                string newOrder = string.Empty;

                                DateTime lastTime = DateTime.UtcNow;
                                // Find order transaction from database.
                                var orderTransaction = await _unitOfWork.OrderPaymentTransactions.GetPaymentTransactionByOrderId(order?.Id);

                                // Update order information.
                                order.PaymentMethodId = EnumPaymentMethod.VNPay;
                                order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                                order.StatusId = EnumOrderStatus.Processing;
                                order.LastSavedTime = lastTime;
                                newOrder = order.ToJsonWithCamelCase();
                                await _unitOfWork.Orders.UpdateAsync(order);

                                // Update payment transaction information.
                                orderTransaction.IsSuccess = true;
                                orderTransaction.LastSavedTime = lastTime;
                                orderTransaction.ResponseData = JsonConvert.SerializeObject(resultFromVnPay);
                                await _unitOfWork.OrderPaymentTransactions.UpdateAsync(orderTransaction);

                                // Add order history.
                                var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), EnumPaymentMethod.VNPay.GetName());
                                var orderHistoryAddModel = new OrderHistory
                                {
                                    OrderId = order.Id,
                                    OldOrrderData = oldOrder,
                                    NewOrderData = newOrder,
                                    ActionName = actionName,
                                    Note = $"{JsonConvert.SerializeObject(resultFromVnPay)}",
                                    CreatedTime = DateTime.UtcNow
                                };

                                await _unitOfWork.OrderHistories.AddAsync(orderHistoryAddModel);
                                await _unitOfWork.SaveChangesAsync();
                                await transaction.CommitAsync();

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

        }
    }
}

using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Commands
{
    public class CancelOrderRequest : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public Guid StoreId { get; set; }
    }

    public class CancelOrderStatusRequestHandle : IRequestHandler<CancelOrderRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        public CancelOrderStatusRequestHandle(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<bool> Handle(CancelOrderRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = await _unitOfWork.Orders
                        .GetOrderDetailDataById(request.OrderId, request.StoreId)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                    if (order == null) return false;

                    var orderItems = order.OrderItems.ToList();
                    foreach (var orderItem in orderItems)
                    {
                        orderItem.StatusId = EnumOrderItemStatus.Canceled;
                    }

                    if (order.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid)
                    {
                        var paymentRefundRequestModel = new PaymentRefundRequestModel
                        {
                            OrderId = order.Id,
                            StoreId = order.StoreId.Value
                        };

                        // Refund payment (Momo, VNPay, Visa/Master card)
                        switch (order.PaymentMethodId)
                        {
                            case EnumPaymentMethod.MoMo:
                                {
                                    paymentRefundRequestModel.PaymentMethod = RefundPaymentMethodConstants.MOMO;
                                    break;
                                }

                            case EnumPaymentMethod.VNPay:
                                {
                                    paymentRefundRequestModel.PaymentMethod = RefundPaymentMethodConstants.VNPAY_WALLER;
                                    break;
                                }

                            default: break;
                        }

                        if (!string.IsNullOrWhiteSpace(paymentRefundRequestModel.PaymentMethod))
                        {
                            await _paymentService.PaymentRefundAsync(paymentRefundRequestModel);
                        }

                        order.OrderPaymentStatusId = EnumOrderPaymentStatus.Refunded;
                    }

                    order.StatusId = EnumOrderStatus.Canceled;
                    await _unitOfWork.SaveChangesAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
        }
    }
}
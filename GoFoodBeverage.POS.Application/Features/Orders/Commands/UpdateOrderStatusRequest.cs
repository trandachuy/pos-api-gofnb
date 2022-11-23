using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.POS.Application.Features.OrderSessions.Queries;
using GoFoodBeverage.POS.Application.Features.Payments.Commands;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;
using GoFoodBeverage.Services.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusRequest : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public EnumOrderStatus OrderStatusId { get; set; }

        public string Reason { get; set; }
    }

    public class UpdateOrderStatusRequestHandler : IRequestHandler<UpdateOrderStatusRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        private readonly IDeliveryService _deliveryService;
        public IHubContext<KitchenSessionHub> _hubContext;
        private readonly IAhamoveService _ahamoveService;

        public UpdateOrderStatusRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IOrderService orderService,
            IDeliveryService deliveryService,
            IHubContext<KitchenSessionHub> hubContext,
            IAhamoveService ahamoveService)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _orderService = orderService;
            _hubContext = hubContext;
            _deliveryService = deliveryService;
            _ahamoveService = ahamoveService;
        }

        public async Task<bool> Handle(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                            .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                            .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                            .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                            .Include(x => x.OrderItems).ThenInclude(i => i.OrderComboItem).ThenInclude(o => o.OrderComboProductPriceItems)
                            .Include(x => x.OrderFees)
                            .Include(x => x.OrderSessions)
                            .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.BranchId == loggedUser.BranchId);
            var oldOrder = order.ToJsonWithCamelCase();
            var orderItems = order.OrderItems.ToList();
            var orderSessions = order?.OrderSessions?.ToList();
            if (request.OrderStatusId == EnumOrderStatus.Canceled && order.StatusId != EnumOrderStatus.Draft)
            {
                await _orderService.CalMaterialQuantity(order.Id, true, false, EnumInventoryHistoryAction.CancelOrder);
            }

            switch (request.OrderStatusId)
            {
                case EnumOrderStatus.Completed:
                    order.StatusId = EnumOrderStatus.Completed;

                    /// Update order item status
                    foreach (var orderItem in orderItems.Where(o => o.StatusId != EnumOrderItemStatus.Canceled))
                    {
                        orderItem.StatusId = EnumOrderItemStatus.Completed;

                        if (orderItem.IsCombo)
                        {
                            foreach (var item in orderItem.OrderComboItem.OrderComboProductPriceItems)
                            {
                                item.StatusId = EnumOrderItemStatus.Completed;
                            }
                        }
                    }

                    _unitOfWork.OrderItems.UpdateRange(orderItems);

                    /// Update order session status
                    foreach (var session in orderSessions)
                    {
                        session.StatusId = EnumOrderSessionStatus.Completed;
                    }

                    _unitOfWork.OrderSessions.UpdateRange(orderSessions);
                    await _orderService.CalculatePoint(order, loggedUser.StoreId);
                    break;
                case EnumOrderStatus.Returned:
                    break;
                case EnumOrderStatus.Canceled:
                    order.StatusId = EnumOrderStatus.Canceled;

                    foreach (var orderItem in orderItems)
                    {
                        orderItem.StatusId = EnumOrderItemStatus.Canceled;

                        if (orderItem.IsCombo)
                        {
                            foreach (var item in orderItem.OrderComboItem.OrderComboProductPriceItems)
                            {
                                item.StatusId = EnumOrderItemStatus.Canceled;
                            }
                        }
                    }

                    _unitOfWork.OrderItems.UpdateRange(orderItems);

                    /// Cancel Ahamove order
                    if (order.AhamoveOrderId != null)
                    {
                        var ahamoveConfig = await _unitOfWork.DeliveryConfigs
                            .Find(dc => dc.StoreId == order.StoreId && dc.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove)
                            .FirstOrDefaultAsync(cancellationToken);
                        // Get ahamove token
                        var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
                        {
                            Mobile = ahamoveConfig.PhoneNumber,
                            Name = ahamoveConfig.Name,
                            ApiKey = ahamoveConfig.ApiKey,
                            Address = ahamoveConfig.Address,
                        };
                        var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);
                        var orderDetailAhamove = await _ahamoveService.GetOrderDetailAsync(infoToken.Token, order.AhamoveOrderId);
                        // Check ahamove order status
                        var ahamoveOrderStatuses = new List<string> { AhamoveOrderStatusConstants.ASSIGNING, AhamoveOrderStatusConstants.ACCEPTED, AhamoveOrderStatusConstants.CONFIRMING, AhamoveOrderStatusConstants.PAYING, AhamoveOrderStatusConstants.IDLE };
                        if (ahamoveOrderStatuses.Contains(orderDetailAhamove.Status))
                        {
                            var orderDeliveryTransaction = new OrderDeliveryTransaction()
                            {
                                StoreId = order.StoreId,
                                OrderId = order.Id,
                                DeliveryMethodId = order.DeliveryMethodId,
                                RequestTime = DateTime.UtcNow,
                            };
                            await _unitOfWork.OrderDeliveryTransactions.AddAsync(orderDeliveryTransaction);
                            var response = await _deliveryService.CancelOrderAhamoveAsync(order.AhamoveOrderId, order.StoreId.Value, request.Reason);
                            if (response)
                            {
                                orderDeliveryTransaction.Status = AhamoveOrderStatusConstants.CANCELLED;
                            }
                            orderDeliveryTransaction.ResponseData = response.ToString();
                        }
                    }

                    if (order.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid)
                    {
                        // Refund payment (Momo, VNPay, Visa/Master card)
                        var createPaymentRefund = new CreatePaymentRefundRequest()
                        {
                            StoreId = loggedUser.StoreId.Value,
                            OrderId = order.Id,
                        };

                        switch (order.PaymentMethodId)
                        {
                            case EnumPaymentMethod.MoMo:
                                {
                                    createPaymentRefund.PaymentMethod = RefundPaymentMethodConstants.MOMO;
                                    break;
                                }

                            case EnumPaymentMethod.VNPay:
                                {
                                    createPaymentRefund.PaymentMethod = RefundPaymentMethodConstants.VNPAY_WALLER;
                                    break;
                                }

                            default: break;
                        }

                        if (!string.IsNullOrWhiteSpace(createPaymentRefund.PaymentMethod))
                        {
                            await _mediator.Send(createPaymentRefund, cancellationToken);
                        }

                        order.OrderPaymentStatusId = EnumOrderPaymentStatus.Refunded;
                    }
                    break;
                case EnumOrderStatus.ToConfirm:
                    break;
                case EnumOrderStatus.Processing:
                    // TODO: Create new order delivery transaction with default status
                    var deliveryMethod = await _unitOfWork.DeliveryMethods.Find(dm => dm.Id == order.DeliveryMethodId).FirstOrDefaultAsync();
                    if (order.StatusId == EnumOrderStatus.ToConfirm && order.PlatformId == EnumPlatform.GoFnBApp.ToGuid() && deliveryMethod.EnumId == EnumDeliveryMethod.AhaMove)
                    {
                        var orderInfoAhamove = await CreateOrderAhaMoveAsync(order, cancellationToken);
                        order.AhamoveOrderId = orderInfoAhamove.OrderId;
                        // TODO: Update order delivery transaction with response status
                    }
                    order.StatusId = EnumOrderStatus.Processing;
                    break;
                case EnumOrderStatus.Delivering:
                    break;
            }

            var newOrder = order.ToJsonWithCamelCase();

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Order,
                ActionType = request.OrderStatusId == EnumOrderStatus.Canceled ? EnumActionType.Cancelled : EnumActionType.UpdateStatus,
                ObjectId = order.Id,
                ObjectName = order.Code
            });

            //Save Order History
            var actionName = request.OrderStatusId == EnumOrderStatus.Canceled ? EnumOrderHistoryActionName.Cancel.GetName() : request.OrderStatusId.GetName();
            await _orderService.SaveOrderHistoryAsync(order.Id, oldOrder, newOrder, actionName, null, request.Reason);

            ///Get list kitchen session via signalR hub
            await GetKitchenOrderSessionsAsync(request.OrderStatusId, order.Id, loggedUser, cancellationToken);

            return true;
        }

        /// <summary>
        /// Get kitchen order session list with signalR
        /// </summary>
        /// <param name="request"></param>
        private async Task GetKitchenOrderSessionsAsync(EnumOrderStatus orderStatus, Guid orderId, LoggedUserModel loggedUser, CancellationToken cancellationToken)
        {
            var kitchenOrderSessions = new GetKitchenOrderSessionsInStoreBranchRequest();
            var kitchenOrderSessionsResult = await _mediator.Send(kitchenOrderSessions, cancellationToken);

            string groupName = loggedUser.BranchId.Value.ToString();
            string kitchenOrderSessionsObject = kitchenOrderSessionsResult.ToJsonWithCamelCase();

            /// Send data to client via signalR
            if (orderStatus == EnumOrderStatus.Completed)
            {
                await _hubContext.Clients.Group(groupName).SendAsync(KitchenConstants.KITCHEN_RECEIVER, kitchenOrderSessionsObject, orderId, cancellationToken: cancellationToken);
            }
            else
            {
                await _hubContext.Clients.Group(groupName).SendAsync(KitchenConstants.KITCHEN_RECEIVER, kitchenOrderSessionsObject, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Create order ahamove when choose ahamove delivery method
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<CreateOrderAhamoveResponseModel> CreateOrderAhaMoveAsync(Order order, CancellationToken cancellationToken)
        {
            var ahamoveConfig = await _unitOfWork.DeliveryConfigs.Find(dc => dc.StoreId == order.StoreId && dc.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove).FirstOrDefaultAsync();

            var paymentMethod = order.OrderPaymentStatusId != EnumOrderPaymentStatus.Paid ? AhamoveDeliveryConfigConstants.PAYMENT_BALANCE : AhamoveDeliveryConfigConstants.PAYMENT_CASH;

            var senderAddressDetail = new CreateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Name = order.OrderDelivery.SenderName,
                Phone = order.OrderDelivery.SenderPhone,
                Address = order.OrderDelivery.SenderAddress,
                Lat = order.OrderDelivery.SenderLat.Value,
                Lng = order.OrderDelivery.SenderLng.Value,
            };

            var receiverAddressDetail = new CreateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Name = order.OrderDelivery.ReceiverName,
                Phone = order.OrderDelivery.ReceiverPhone,
                Address = order.OrderDelivery.ReceiverAddress,
                Lat = order.OrderDelivery.ReceiverLat.Value,
                Lng = order.OrderDelivery.ReceiverLng.Value,
                Remarks = order.Note,
                Cod = paymentMethod == AhamoveDeliveryConfigConstants.PAYMENT_BALANCE ? 0 : (double?)order.TotalCost,
            };

            var products = new List<CreateOrderAhamoveRequestModel.AhamoveProductDto>();

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                var productPriceIds = order.OrderItems.Select(c => c.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => productPriceIds.Any(id => id == p.Id))
                    .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                    .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                    .Include(p => p.ProductPriceMaterials).ThenInclude(ppm => ppm.Material)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                foreach (var cartItem in order.OrderItems)
                {
                    if (!cartItem.IsCombo)
                    {
                        var product = new CreateOrderAhamoveRequestModel.AhamoveProductDto()
                        {
                            Id = cartItem.OrderComboItem.ComboName,
                            Name = cartItem.OrderComboItem.ComboName,
                            Amount = 1,
                            Price = cartItem.OrderComboItem.SellingPrice,
                        };
                        products.Add(product);
                    }
                    else
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        var item = order.OrderItems.FirstOrDefault(i => i.ProductPriceId == cartItem.ProductPriceId);
                        if (item == null || productPrice == null) continue;

                        var product = new CreateOrderAhamoveRequestModel.AhamoveProductDto()
                        {
                            Id = productPrice.Product.Name,
                            Name = productPrice.Product.Name,
                            Amount = cartItem.Quantity,
                            Price = cartItem.PriceAfterDiscount.Value,
                        };
                        products.Add(product);
                    }
                }
            }

            var estimateOrderAhamoveRequest = new CreateOrderAhamoveRequestModel()
            {
                PaymentMethod = paymentMethod,
                SenderAddress = senderAddressDetail,
                ReceiverAddress = receiverAddressDetail,
                Products = products
            };

            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahamoveConfig.PhoneNumber,
                Name = ahamoveConfig.Name,
                ApiKey = ahamoveConfig.ApiKey,
                Address = ahamoveConfig.Address,
            };
            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);

            var orderInfoAhamove = await _ahamoveService.CreateOrderAsync(infoToken.Token, estimateOrderAhamoveRequest);

            return orderInfoAhamove;
        }
    }
}

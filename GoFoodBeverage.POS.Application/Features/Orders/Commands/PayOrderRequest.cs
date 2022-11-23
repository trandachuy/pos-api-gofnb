using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;
using Microsoft.AspNetCore.SignalR;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.POS.Application.Features.OrderSessions.Queries;
using GoFoodBeverage.Common.Constants;
using System.Linq;
using GoFoodBeverage.Services.Hubs;

namespace GoFoodBeverage.POS.Application.Features.Orders.Commands
{
    public class PayOrderRequest : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public EnumPaymentMethod PaymentMethod { get; set; }

        public decimal? ReceivedAmount { get; set; }

        public decimal? Change { get; set; }
    }

    public class CreateNormalPaymentRequestHandler : IRequestHandler<PayOrderRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        public IHubContext<KitchenSessionHub> _hubContext;

        public CreateNormalPaymentRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IOrderService orderService,
            IHubContext<KitchenSessionHub> hubContext
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _orderService = orderService;
            _hubContext = hubContext;
        }

        public async Task<bool> Handle(PayOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                         .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                         .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                         .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                         .Include(x => x.OrderFees)
                         .Include(x => x.DeliveryMethod)
                         .Include(x => x.OrderSessions)
                         .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                         .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            ThrowError.Against(order == null, "Cannot find order information");
          
            var oldOrder = order.ToJsonWithCamelCase();
            var storeConfig = await _unitOfWork.Stores.GetStoreConfigAsync(loggedUser.StoreId.Value);
            if (!storeConfig.IsPaymentLater)
            {
                await _orderService.CalMaterialQuantity(order.Id, false, false, EnumInventoryHistoryAction.CreateOrder);
            }

            switch (request.PaymentMethod)
            {
                case EnumPaymentMethod.Cash:
                    order.PaymentMethodId = EnumPaymentMethod.Cash;
                    order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;

                    var orderStatus = await _orderService.GetOrderStatusAsync(order.OrderTypeId, order?.DeliveryMethod?.EnumId);
                    order.StatusId = orderStatus.OrderStatus;
                    if (orderStatus.OrderStatus == EnumOrderStatus.Completed)
                    {
                        /// Update order item status
                        var orderItems = order.OrderItems.ToList();
                        foreach (var orderItem in orderItems)
                        {
                            orderItem.StatusId = EnumOrderItemStatus.Completed;
                        }

                        _unitOfWork.OrderItems.UpdateRange(orderItems);

                        /// Update order session status
                        var orderSessions = order?.OrderSessions?.ToList();
                        foreach (var session in orderSessions)
                        {
                            session.StatusId = EnumOrderSessionStatus.Completed;
                        }

                        _unitOfWork.OrderSessions.UpdateRange(orderSessions);
                    }

                    order.ReceivedAmount = request.ReceivedAmount ?? 0;
                    order.Change = request.Change ?? 0;
                    await _orderService.CalculatePoint(order, loggedUser.StoreId);

                    break;

                case EnumPaymentMethod.CreditDebitCard:
                    order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                    order.PaymentMethodId = EnumPaymentMethod.CreditDebitCard;
                    //Todo: implement code for CreditDebitCard
                    break;

                case EnumPaymentMethod.MoMo:
                    order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                    order.PaymentMethodId = EnumPaymentMethod.MoMo;
                    //Todo: implement code for MoMo
                    break;

                case EnumPaymentMethod.ZaloPay:
                    order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                    order.PaymentMethodId = EnumPaymentMethod.ZaloPay;
                    //Todo: implement code for ZaloPay
                    break;

                default:
                    break;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            //Save Order History
            var newOrder = order.ToJsonWithCamelCase();
            var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), request.PaymentMethod.GetName());
            await _orderService.SaveOrderHistoryAsync(order.Id,oldOrder,newOrder, actionName, null, null);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Order,
                ActionType = EnumActionType.PaymentStatus,
                ObjectId = order.Id,
                ObjectName = order.Code,
            });

            ///Get list kitchen session via signalR hub
            await GetKitchenOrderSessionsAsync(loggedUser, cancellationToken);

            return true;
        }

        /// <summary>
        /// Get kitchen order session list with signalR
        /// </summary>
        /// <param name="request"></param>
        private async Task GetKitchenOrderSessionsAsync(LoggedUserModel loggedUser, CancellationToken cancellationToken)
        {
            var kitchenOrderSessions = new GetKitchenOrderSessionsInStoreBranchRequest();
            var kitchenOrderSessionsResult = await _mediator.Send(kitchenOrderSessions, cancellationToken);

            string groupName = loggedUser.BranchId.Value.ToString();
            string jsonObject = kitchenOrderSessionsResult.ToJsonWithCamelCase();

            /// Send data to client via signalR
            await _hubContext.Clients.Group(groupName).SendAsync(KitchenConstants.KITCHEN_RECEIVER, jsonObject, cancellationToken: cancellationToken);
        }
    }
}

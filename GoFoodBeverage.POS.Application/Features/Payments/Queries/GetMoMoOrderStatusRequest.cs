using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.MoMo.Model;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Payments.Queries
{
    public class GetMoMoOrderStatusRequest : IRequest<QueryStatusResponseModel>
    {
        /// <summary>
        /// Request ID, unique for each request, MoMo's partner uses the requestId field for idempotency control
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Order amount in VND (0 VNĐ or greater)
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Partner Transaction ID
        /// Regex: ^[0-9a-zA-Z] ([-_.]*[0 - 9a - zA - Z]+)*$
        /// </summary>
        public string OrderId { get; set; }
    }

    public class GetOrderStatusRequestHandler : IRequestHandler<GetMoMoOrderStatusRequest, QueryStatusResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMoMoPaymentService _momoPaymentService;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        private readonly IKitchenService _kitchenService;
        private readonly IMediator _mediator;

        public GetOrderStatusRequestHandler(
            IUnitOfWork unitOfWork,
            IMoMoPaymentService momoPaymentService,
            IUserProvider userProvider,
            IOrderService orderService,
            IKitchenService kitchenService,
            IMediator mediator
        )
        {
            _unitOfWork = unitOfWork;
            _momoPaymentService = momoPaymentService;
            _userProvider = userProvider;
            _orderService = orderService;
            _kitchenService = kitchenService;
            _mediator = mediator;
        }

        public async Task<QueryStatusResponseModel> Handle(GetMoMoOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var paymentConfig = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.MoMo);
            var storeMomoPaymentConfig = new PartnerMoMoPaymentConfigModel()
            {
                PartnerCode = paymentConfig.PartnerCode,
                AccessKey = paymentConfig.AccessKey,
                SecretKey = paymentConfig.SecretKey,
            };

            var requestData = new QueryStatusRequestModel()
            {
                RequestId = request.RequestId,
                Amount = request.Amount,
                OrderId = request.OrderId,
            };

            // Payment status from MOMO response
            var momoPaymentStatus = await _momoPaymentService.QueryStatusAsync(storeMomoPaymentConfig, requestData);

            // Update transaction logging
            var orderPaymentUpdate = await _unitOfWork.OrderPaymentTransactions.GetOrderPaymentTransactionById(Guid.Parse(momoPaymentStatus.RequestId));
            orderPaymentUpdate.ResponseData = momoPaymentStatus.Message;
            orderPaymentUpdate.IsSuccess = momoPaymentStatus.IsSuccess;
            orderPaymentUpdate.TransId = momoPaymentStatus.TransId;
            _unitOfWork.OrderPaymentTransactions.Update(orderPaymentUpdate);

            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
             .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
             .Include(x => x.OrderFees)
             .Include(x => x.AreaTable).ThenInclude(x => x.Area)
             .FirstOrDefaultAsync(o => o.Id == orderPaymentUpdate.OrderId.Value);

            var oldOrder = order.ToJsonWithCamelCase();
            if (momoPaymentStatus.IsSuccess)
            {
                order.PaymentMethodId = EnumPaymentMethod.MoMo;

                // Handle update order and payment status
                var orderStatus = await _orderService.GetOrderStatusAsync(order.OrderTypeId, null);
                order.StatusId = orderStatus.OrderStatus;
                order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
            }
            else
            {
                // If the payment response is failed => change order status to Draft
                order.OrderPaymentStatusId = null;
                order.StatusId = EnumOrderStatus.Draft;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            var newOrder = order.ToJsonWithCamelCase();
            var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), EnumPaymentMethod.MoMo.GetName());
            await _orderService.SaveOrderHistoryAsync(orderPaymentUpdate.OrderId.Value, oldOrder, newOrder, actionName, null, null);
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

            return momoPaymentStatus;
        }
    }
}

using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.OrderSessions.Commands
{
    public class UpdateOrderSessionStatusRequest : IRequest<bool>
    {
        public Guid SessionId { get; set; }

        public string OrderCode { get; set; }
    }

    public class UpdateOrderSessionStatusRequestHandler : IRequestHandler<UpdateOrderSessionStatusRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKitchenService _kitchenService;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateOrderSessionStatusRequestHandler(
            IUnitOfWork unitOfWork,
            IKitchenService kitchenService,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _kitchenService = kitchenService;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateOrderSessionStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var currentSession = await _unitOfWork.OrderSessions.GetOrderSessionByIdAsync(request.SessionId, loggedUser.StoreId);
            var orderItems = await _unitOfWork.OrderItems.GetOrderItemByOrderSessionIdAsync(request.SessionId, loggedUser.StoreId);

            var updatedSession = UpdateOrderSession(currentSession, orderItems);

            _unitOfWork.OrderSessions.Update(updatedSession);

            await _unitOfWork.SaveChangesAsync();

            await _kitchenService.GetOrderCodeFromKitchenAsync(request.OrderCode, loggedUser, cancellationToken);

            return true;
        }

        private OrderSession UpdateOrderSession(OrderSession orderSession, List<OrderItem> orderItems)
        {
            orderSession.StatusId = EnumOrderSessionStatus.Completed;
            foreach (var orderItem in orderItems)
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

            return orderSession;
        }
    }
}

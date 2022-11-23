using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Services.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.OrderSessions.Commands
{
    public class UpdateOrderItemStatusRequest : IRequest<bool>
    {
        public Guid OrderItemId { get; set; }

        public Guid SessionId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime? CreatedTime { get; set; }

        public Guid? OrderComboProductPriceItemId { get; set; }

        public string OrderCode { get; set; }
    }

    public class UpdateOrderItemStatusRequestHandler : IRequestHandler<UpdateOrderItemStatusRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        public IHubContext<KitchenSessionHub> _hubContext;
        private readonly IKitchenService _kitchenService;

        public UpdateOrderItemStatusRequestHandler(IUnitOfWork unitOfWork, IUserProvider userProvider, IHubContext<KitchenSessionHub> hubContext, IKitchenService kitchenService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _hubContext = hubContext;
            _kitchenService = kitchenService;
        }

        public async Task<bool> Handle(UpdateOrderItemStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            if (request.OrderComboProductPriceItemId.HasValue)
            {
                var orderItem = await _unitOfWork.
                    OrderComboProductPriceItems.
                    GetByIdAsync(request.OrderComboProductPriceItemId.Value, loggedUser.StoreId);

                if (orderItem != null)
                {
                    orderItem.StatusId = EnumOrderItemStatus.Completed;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                var currentOrderItem = await _unitOfWork.OrderItems
                .GetOrderItemForUpdateStatusAsync(request.OrderItemId, request.SessionId, request.ProductId, request.CreatedTime, loggedUser.StoreId);

                if (currentOrderItem != null)
                {
                    currentOrderItem.StatusId = EnumOrderItemStatus.Completed;

                    _unitOfWork.OrderItems.Update(currentOrderItem);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            await _kitchenService.GetOrderCodeFromKitchenAsync(request.OrderCode, loggedUser, cancellationToken);

            return true;
        }
    }
}

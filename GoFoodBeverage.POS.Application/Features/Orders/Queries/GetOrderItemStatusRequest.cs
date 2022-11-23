using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class CheckPreparedStatusForOrderItemRequest : IRequest<CheckPreparedStatusForOrderItemResponse>
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
    }

    public class CheckPreparedStatusForOrderItemResponse
    {
        public int TotalRemainingItem { get; set; }

        public int TotalItemCompleted { get; set; }
    }

    public class GetOrderItemStatusRequestHandler : IRequestHandler<CheckPreparedStatusForOrderItemRequest, CheckPreparedStatusForOrderItemResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetOrderItemStatusRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckPreparedStatusForOrderItemResponse> Handle(CheckPreparedStatusForOrderItemRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            IList<OrderItem> orderItemEntity = await _unitOfWork.OrderItems
                .GetAll()
                .Where(oi => oi.StoreId == loggedUser.StoreId && oi.OrderId == request.OrderId && oi.ProductId == request.ProductId)
                .AsNoTracking()
                .ToListAsync();

            int totalNumberOfItemsCompleted = orderItemEntity.Count(oi => oi.ProductId == request.ProductId && oi.StatusId == EnumOrderItemStatus.Completed);
            int available = orderItemEntity.Count - totalNumberOfItemsCompleted;

            var response = new CheckPreparedStatusForOrderItemResponse()
            {
                TotalRemainingItem = available,
                TotalItemCompleted = totalNumberOfItemsCompleted
            };

            return response;
        }
    }
}

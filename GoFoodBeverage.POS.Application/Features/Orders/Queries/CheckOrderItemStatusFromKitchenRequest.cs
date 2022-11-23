using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.POS.Models.Order;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class CheckOrderItemStatusFromKitchenRequest : IRequest<CheckOrderItemStatusFromKitchenResponse>
    {
        public Guid OrderId { get; set; }
    }

    public class CheckOrderItemStatusFromKitchenResponse
    {
        public bool IsAllowToRemoveOrderItem { get; set; } = true;
    }

    public class CheckOrderItemStatusFromKitchenRequestHandler : IRequestHandler<CheckOrderItemStatusFromKitchenRequest, CheckOrderItemStatusFromKitchenResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public CheckOrderItemStatusFromKitchenRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<CheckOrderItemStatusFromKitchenResponse> Handle(CheckOrderItemStatusFromKitchenRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var orderItems = await _unitOfWork.OrderItems
                .Find(o => o.StoreId == loggedUser.StoreId && o.OrderId == request.OrderId)
                .Include(o => o.OrderComboItem).ThenInclude(c => c.OrderComboProductPriceItems)
                .AsNoTracking()
                .ProjectTo<CheckOrderItemModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            bool isAllowToRemoveOrderItem = true;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.StatusId == EnumOrderItemStatus.Completed)
                {
                    isAllowToRemoveOrderItem = false;
                    break;
                }
                else
                {
                    if (orderItem.IsCombo)
                    {
                        foreach (var item in orderItem?.OrderComboItem?.OrderComboProductPriceItems)
                        {
                            if (item.StatusId == EnumOrderItemStatus.Completed)
                            {
                                isAllowToRemoveOrderItem = false;
                                break;
                            }
                        }
                    }
                }
            }

            var response = new CheckOrderItemStatusFromKitchenResponse()
            {
                IsAllowToRemoveOrderItem = isAllowToRemoveOrderItem
            };

            return response;
        }
    }
}
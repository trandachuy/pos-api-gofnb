using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.POS.Models.OrderSession;

namespace GoFoodBeverage.POS.Application.Features.OrderSessions.Queries
{
    public class GetKitchenOrderSessionsInStoreBranchRequest : IRequest<GetKitchenOrderSessionsInStoreBranchResponse>
    {
    }

    public class GetKitchenOrderSessionsInStoreBranchResponse
    {
        public List<KitchenOrderSessionModel> KitchenOrderSessions { get; set; }
    }

    public class GetKitchenOrderSessionsInStoreBranchRequestHandler : IRequestHandler<GetKitchenOrderSessionsInStoreBranchRequest, GetKitchenOrderSessionsInStoreBranchResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetKitchenOrderSessionsInStoreBranchRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetKitchenOrderSessionsInStoreBranchResponse> Handle(GetKitchenOrderSessionsInStoreBranchRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var startDateCompare = DateTime.Today;
            var endDateCompare = DateTime.Today.AddDays(1).AddSeconds(-1);

            var listOrderSessions = await _unitOfWork.OrderSessions
                .GetKitchenOrderSessionInStore(loggedUser.StoreId.Value, loggedUser.BranchId.Value)
                .Where(o => o.CreatedTime.Value.CompareTo(startDateCompare) >= 0
                    && endDateCompare.CompareTo(o.CreatedTime.Value) >= 0
                    && o.StatusId != EnumOrderSessionStatus.Completed
                    && o.Order.StatusId != EnumOrderStatus.Draft)
                .OrderBy(o => o.CreatedTime.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var kitchenOrderSessionResponse = new List<KitchenOrderSessionModel>();

            listOrderSessions.ForEach(orderSession =>
            {
                var session = new KitchenOrderSessionModel
                {
                    SessionId = orderSession.Id,
                    SessionCode = $"#{orderSession.Order.StringCode}",
                    OrderId = orderSession.OrderId,
                    OrderCode = orderSession.Order.StringCode,
                    OrderTypeId = orderSession.Order.OrderTypeId,
                    OrderTypeName = orderSession.Order.OrderTypeId.GetName(),
                    AreaName = orderSession.Order.AreaTable?.Area?.Name,
                    TableName = orderSession.Order.AreaTable?.Name,
                    CreatedTime = orderSession.CreatedTime,
                    TotalOrderItem = CountOrderItems(orderSession.OrderItems),
                    TotalOrderItemCanceled = CountOrderItemsByStatus(orderSession.OrderItems, EnumOrderItemStatus.Canceled),
                    TotalOrderItemCompleted = CountOrderItemsByStatus(orderSession.OrderItems, EnumOrderItemStatus.Completed),
                    OrderItems = ConvertOrderItems(orderSession, loggedUser.StoreId),
                };

                MergeOrderItems(session);

                kitchenOrderSessionResponse.Add(session);
            });

            var response = IndexOrderSession(kitchenOrderSessionResponse);

            return new GetKitchenOrderSessionsInStoreBranchResponse()
            {
                KitchenOrderSessions = response
            };
        }

        private static List<KitchenOrderSessionModel> IndexOrderSession(List<KitchenOrderSessionModel> orderSessions)
        {
            var result = new List<KitchenOrderSessionModel>();
            var orderSessionsGroupByOrder = orderSessions.OrderBy(s => s.CreatedTime).GroupBy(s => s.OrderCode);
            foreach (var group in orderSessionsGroupByOrder)
            {
                var sessions = group.ToList();
                if (!sessions.Any()) continue;

                if (sessions.Count > 1)
                {
                    sessions.ForEach(session =>
                    {
                        var index = sessions.IndexOf(session) + 1;
                        session.SessionCode = $"{session.SessionCode}";
                        session.SessionIndex = $"({index})";
                        result.Add(session);
                    });
                }
                else
                {
                    var session = sessions.First();
                    result.Add(session);
                }
            }

            result = result.OrderBy(s => s.CreatedTime).ToList();

            return result;
        }

        private static void MergeOrderItems(KitchenOrderSessionModel request)
        {
            var result = new List<KitchenOrderSessionModel.OrderItemDto>();

            // Make sure the item has a status of new, it will be added at the top.
            foreach (var orderItem in request.OrderItems.OrderBy(x => x.StatusId))
            {
                var existed = GetProductItemDuplicated(orderItem, result);
                if (existed == null)
                {
                    result.Add(orderItem);
                }
                else
                {
                    existed.CurrentQuantity += orderItem.CurrentQuantity;
                    existed.DefaultQuantity += orderItem.DefaultQuantity;
                }
            }

            // The items with the smallest quantity will be at the top.
            request.OrderItems = result.OrderBy(x => x.StatusId).ToList();
        }

        private static KitchenOrderSessionModel.OrderItemDto GetProductItemDuplicated(
            KitchenOrderSessionModel.OrderItemDto item,
            List<KitchenOrderSessionModel.OrderItemDto> result
        )
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = item.ProductPriceId == existed.ProductPriceId;
                bool isStatusDuplicated = item.StatusId == existed.StatusId;
                if ((item.StatusId == EnumOrderItemStatus.Completed && existed.StatusId == EnumOrderItemStatus.New) ||
                   (item.StatusId == EnumOrderItemStatus.New && existed.StatusId == EnumOrderItemStatus.Completed))
                {
                    isStatusDuplicated = true;
                }
                var isOptionDuplicated = item.OrderItemOptions.All(o => existed.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.OrderItemToppings.All(o => existed.OrderItemToppings.Any(e => e.ToppingName == o.ToppingName && e.Quantity == o.Quantity)) && item.OrderItemToppings.Count == existed.OrderItemToppings.Count;
                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated && isStatusDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }

        /// <summary>
        /// This method is used to convert data from entity to the model data.
        /// Each item in the order session can be a combo or a normal item.
        /// </summary>
        /// <param name="orderSession">Items in the current session will be converted.</param>
        /// <returns></returns>
        private List<KitchenOrderSessionModel.OrderItemDto> ConvertOrderItems(OrderSession orderSession, Guid? storeId)
        {
            var orderItems = new List<KitchenOrderSessionModel.OrderItemDto>();
            var optionLevelIds = orderSession.OrderItems.SelectMany(item => item.OrderItemOptions.Select(ol => ol.OptionLevelId));
            var optionLevels = _unitOfWork.OptionLevels
                .Find(ol => ol.StoreId == storeId && optionLevelIds.Any(olid => olid == ol.Id))
                .AsNoTracking()
                .ToListAsync()
                .Result;

            foreach (OrderItem orderItem in orderSession.OrderItems)
            {
                if (orderItem.IsCombo)
                {
                    // If the item is a combo, get data in the OrderComboItem table and convert it to the type of OrderItemDto.
                    foreach (var item in orderItem.OrderComboItem.OrderComboProductPriceItems)
                    {
                        var anOrderItem = new KitchenOrderSessionModel.OrderItemDto
                        {
                            OrderItemId = orderItem.Id,
                            ProductId = item.ProductPrice.ProductId,
                            ProductName = item.ProductPrice?.Product?.Name,
                            ProductPriceId = item.ProductPriceId,
                            ProductPriceName = item.ProductPrice.PriceName,
                            Notes = orderItem.Notes,
                            DefaultQuantity = 1,
                            // Check the quantity in each item in this combo.
                            CurrentQuantity = item.StatusId == EnumOrderItemStatus.Completed ? 0 : 1,
                            StatusId = item.StatusId,
                            CreatedTime = orderItem.CreatedTime.Value,
                            // The id is used to update the status for this item in the table OrderComboProductPriceItems.
                            OrderComboProductPriceItemId = item?.Id,
                            OrderItemOptions = item.
                            OrderItemOptions?.
                            Select(option => new KitchenOrderSessionModel.OrderItemDto.OrderItemOptionsDto
                            {
                                OptionId = option.Id,
                                OptionLevelId = option.OptionLevelId,
                                OptionName = option.OptionName,
                                OptionLevelName = option.OptionLevelName,
                                IsSetDefault = optionLevels.FirstOrDefault(item => (item.Id == option.OptionLevelId)).IsSetDefault,
                            }).ToList(),
                            OrderItemToppings = item.
                            OrderItemToppings?.
                            Select(topping => new KitchenOrderSessionModel.OrderItemDto.OrderItemToppingsDto
                            {
                                ToppingId = topping.Id,
                                ToppingName = topping.ToppingName,
                                ToppingValue = topping.ToppingValue,
                                Quantity = topping.Quantity,
                            }).ToList(),

                        };
                        orderItems.Add(anOrderItem);
                    }
                }
                else
                {
                    // We will use this id in the table OrderItemId to update the item status.
                    // See more in GoFoodBeverage.POS.Application.Features.OrderSessions.Commands.UpdateOrderItemStatusRequestHandler
                    var anOrderItem = new KitchenOrderSessionModel.OrderItemDto
                    {
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.ProductName,
                        ProductPriceId = orderItem.ProductPriceId,
                        ProductPriceName = orderItem.ProductPrice?.PriceName,
                        Notes = orderItem.Notes,
                        DefaultQuantity = 1,
                        CurrentQuantity = orderItem.StatusId == EnumOrderItemStatus.Completed ? 0 : 1,
                        StatusId = orderItem.StatusId,
                        CreatedTime = orderItem.CreatedTime.Value,
                        OrderItemOptions = orderItem.
                            OrderItemOptions?.
                            Select(option => new KitchenOrderSessionModel.OrderItemDto.OrderItemOptionsDto
                            {
                                OptionId = option.Id,
                                OptionLevelId = option.OptionLevelId,
                                OptionName = option.OptionName,
                                OptionLevelName = option.OptionLevelName,
                                IsSetDefault = optionLevels.FirstOrDefault(item => (item.Id == option.OptionLevelId)).IsSetDefault,
                            }).ToList(),
                        OrderItemToppings = orderItem.
                            OrderItemToppings?.
                            Select(topping => new KitchenOrderSessionModel.OrderItemDto.OrderItemToppingsDto
                            {
                                ToppingId = topping.Id,
                                ToppingName = topping.ToppingName,
                                ToppingValue = topping.ToppingValue,
                                Quantity = topping.Quantity,
                            }).ToList(),

                    };
                    orderItems.Add(anOrderItem);
                }

            }

            return orderItems;
        }

        /// <summary>
        /// This method is used to count all order items.
        /// </summary>
        /// <param name="orderItems">The order items in a session.</param>
        /// <returns>int</returns>
        private int CountOrderItems(ICollection<OrderItem> orderItems)
        {
            int count = 0;

            foreach (var anOrderIteam in orderItems)
            {
                if (anOrderIteam.IsCombo)
                {
                    foreach (var item in anOrderIteam.OrderComboItem.OrderComboProductPriceItems)
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// This method is used to count order items by status.
        /// </summary>
        /// <param name="orderItems">The order items in a session.</param>
        /// <param name="status">The order item status, it can be New, Cancel or Completed</param>
        /// <returns>int</returns>
        private int CountOrderItemsByStatus(ICollection<OrderItem> orderItems, EnumOrderItemStatus status)
        {
            int count = 0;

            foreach (var anOrderIteam in orderItems)
            {
                if (anOrderIteam.IsCombo)
                {
                    foreach (var item in anOrderIteam.OrderComboItem.OrderComboProductPriceItems)
                    {
                        if (item.StatusId == status)
                        {
                            count++;
                        }
                    }
                }
                else
                {
                    if (anOrderIteam.StatusId == status)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}

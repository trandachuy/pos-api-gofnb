using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.POS.Models.Kitchen;
using GoFoodBeverage.POS.Models.OrderSession;
using GoFoodBeverage.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services.Store
{
    public class KitchenService : IKitchenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        public IHubContext<KitchenSessionHub> _hubContext;

        public KitchenService(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IHubContext<KitchenSessionHub> hubContext
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        #region Update Order Item Status
        public async Task UpdateOrderItemStatusAsync(UpdateOrderItemStatusRequestModel request)
        {
            var loggedUser = await _userProvider.ProvideAsync();

            var currentOrderItem = await _unitOfWork.OrderItems
                .GetOrderItemForUpdateStatusAsync(request.OrderItemId, request.SessionId, request.ProductId, request.CreatedTime, loggedUser.StoreId);

            if (currentOrderItem != null)
            {
                var updatedOrderItem = UpdateOrderItem(currentOrderItem);
                _unitOfWork.OrderItems.Update(updatedOrderItem);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private static OrderItem UpdateOrderItem(OrderItem orderItem)
        {
            orderItem.StatusId = EnumOrderItemStatus.Completed;
            orderItem.Quantity = 0;

            return orderItem;
        }
        #endregion

        #region Update Order Session Status
        public async Task UpdateOrderSessionStatusAsync(UpdateOrderSessionStatusRequestModel request)
        {
            var loggedUser = await _userProvider.ProvideAsync();

            var currentSession = await _unitOfWork.OrderSessions.GetOrderSessionByIdAsync(request.SessionId, loggedUser.StoreId);
            var orderItems = await _unitOfWork.OrderItems.GetOrderItemByOrderSessionIdAsync(request.SessionId, loggedUser.StoreId);

            var updatedSession = UpdateOrderSession(currentSession, orderItems);

            _unitOfWork.OrderSessions.Update(updatedSession);

            await _unitOfWork.SaveChangesAsync();
        }

        private OrderSession UpdateOrderSession(OrderSession orderSession, List<OrderItem> orderItems)
        {
            orderSession.StatusId = EnumOrderSessionStatus.Completed;

            foreach (var orderItem in orderItems)
            {
                orderItem.StatusId = EnumOrderItemStatus.Completed;
            }
            _unitOfWork.OrderItems.UpdateRange(orderItems);

            return orderSession;
        }
        #endregion

        /// <summary>
        /// Get kitchen order session list with signalR
        /// </summary>
        /// <param name="request"></param>
        public async Task GetKitchenOrderSessionsAsync(CancellationToken cancellationToken)
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
                    OrderItems = ConvertOrderItems(orderSession),
                };

                MergeOrderItems(session);

                kitchenOrderSessionResponse.Add(session);
            });

            var kitchenOrderSessionsResult = IndexOrderSession(kitchenOrderSessionResponse);
            string groupName = loggedUser.BranchId.Value.ToString();
            string jsonObject = kitchenOrderSessionsResult.ToJsonWithCamelCase();

            /// Send data to client via signalR
            await _hubContext.Clients.Group(groupName).SendAsync(KitchenConstants.KITCHEN_RECEIVER, jsonObject, cancellationToken: cancellationToken);
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

        /// <summary>
        /// This method is used to convert data from entity to the model data.
        /// Each item in the order session can be a combo or a normal item.
        /// </summary>
        /// <param name="orderSession">Items in the current session will be converted.</param>
        /// <returns></returns>
        private List<KitchenOrderSessionModel.OrderItemDto> ConvertOrderItems(OrderSession orderSession)
        {
            var orderItems = new List<KitchenOrderSessionModel.OrderItemDto>();

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
            request.OrderItems = result.OrderBy(x => x.StatusId).ThenBy(x => x.CurrentQuantity).ToList();
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
        /// Get order code from kitchen to cashier with signalR
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="loggedUser"></param>
        /// <param name="cancellationToken"></param>
        public async Task GetOrderCodeFromKitchenAsync(string orderCode, LoggedUserModel loggedUser, CancellationToken cancellationToken)
        {
            string groupName = loggedUser.BranchId.Value.ToString();

            /// Send data to client via signalR
            await _hubContext.Clients.Group(groupName).SendAsync(KitchenConstants.CASHIER_RECEIVER, orderCode, cancellationToken: cancellationToken);
        }
    }
}

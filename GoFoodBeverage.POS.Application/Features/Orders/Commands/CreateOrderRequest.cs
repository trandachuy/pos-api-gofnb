using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using GoFoodBeverage.POS.Application.Features.Products.Queries;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Product;
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
    public class CreateOrderRequest : IRequest<CreateOrderResponse>
    {
        public bool Paid { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? TableId { get; set; }

        public EnumOrderType EnumOrderTypeId { get; set; }

        public EnumPaymentMethod EnumPaymentMethodId { get; set; }

        public EnumDeliveryMethod? DeliveryMethod { get; set; }

        public IEnumerable<Guid> OrderFeeIds { get; set; }

        public List<OrderCartItemRequestModel> CartItems { get; set; }

        public EnumOrderStatus? OrderStatus { get; set; }

        public bool IsDeliveryOrder { get; set; }

        public Guid? DeliveryMethodId { get; set; }

        public decimal? DeliveryFee { get; set; }

        public decimal TotalTax { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public ReceiverAddressDto ReceiverAddress { get; set; }

        public bool IsDraftOrderPublished { get; set; }

        public class ReceiverAddressDto
        {
            public string Address { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }
        }

        public string Note { get; set; }
    }

    public class CreateOrderResponse
    {
        public bool Success { get; set; }

        public DataDto Data { get; set; }

        public string Message { get; set; }

        public class DataDto
        {
            public Guid OrderId { get; set; }
        }
    }

    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMediator _mediator;
        private readonly IOrderService _orderService;
        private readonly IAhamoveService _ahamoveService;
        private readonly MapperConfiguration _mapperConfiguration;
        public IHubContext<KitchenSessionHub> _hubContext;

        public CreateOrderRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMediator mediator,
            IOrderService orderService,
            IAhamoveService ahamoveService,
            MapperConfiguration mapperConfiguration,
            IHubContext<KitchenSessionHub> hubContext
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mediator = mediator;
            _orderService = orderService;
            _mapperConfiguration = mapperConfiguration;
            _ahamoveService = ahamoveService;
            _hubContext = hubContext;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var response = new CreateOrderResponse();
            ThrowError.Against(!request.CartItems.Any(), "Cannot create order");

            var calculatingCartItems = new GetProductCartItemRequest()
            {
                CustomerId = request.CustomerId,
                CartItems = request.CartItems,
                OrderFeeIds = request.OrderFeeIds
            };

            var calculatingCartItemsResult = await _mediator.Send(calculatingCartItems, cancellationToken);

            var newCartItems = await SplitOrderItems(calculatingCartItemsResult.CartItems, request.OrderId, loggedUser.StoreId);
            var saveResult = await SaveOrderAsync(calculatingCartItemsResult, newCartItems, loggedUser, request, cancellationToken, request.IsDraftOrderPublished);

            response.Success = true;
            response.Data = new CreateOrderResponse.DataDto() { OrderId = saveResult.Item1 };

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Order,
                ActionType = request.OrderId.HasValue ? EnumActionType.Edited : EnumActionType.Created,
                ObjectId = saveResult.Item1,
                ObjectName = saveResult.Item2.ToString()
            });
            return response;
        }

        public static void RequestValidation(CreateOrderRequest request)
        {
            ThrowError.BadRequestAgainstNull(request.CustomerId, "Please select customer");
        }

        private async Task<Tuple<Guid, string>> SaveOrderAsync(
            GetProductCartItemResponse calculatingCartItemsResult,
            List<ProductCartItemModel> cartItems,// cart items split the quantity
            LoggedUserModel loggedUser,
            CreateOrderRequest request,
            CancellationToken cancellationToken,
            bool isDraftOrderPublished)
        {
            if (request.OrderId.HasValue)
            {
                var result = await UpdateOrderAsync(calculatingCartItemsResult, cartItems, loggedUser, request, cancellationToken, isDraftOrderPublished);

                return result;
            }
            else
            {
                var result = await CreateOrderAsync(calculatingCartItemsResult, cartItems, loggedUser, request, cancellationToken);

                return result;
            }
        }

        private async Task<Tuple<Guid, string>> CreateOrderAsync(
            GetProductCartItemResponse calculatingCartItemsResult,
            List<ProductCartItemModel> cartItems,
            LoggedUserModel loggedUser,
            CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            var shift = await _unitOfWork.Shifts
                .Find(s => s.StoreId == loggedUser.StoreId && s.Id == loggedUser.ShiftId)
                .Include(s => s.Staff)
                .Select(s => new
                {
                    s.Id,
                    s.Staff
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.BadRequestAgainstNull(shift, "Can not found shift information");

            #region Handle create new an order and related data to this order
            var platformId = _userProvider.GetPlatformId();
            var dataPlatformId = await _unitOfWork.Platforms
                .Find(p => p.Id.ToString() == platformId)
                .AsNoTracking()
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var totalOriginalPrice = calculatingCartItemsResult.OriginalPrice;
            var totalDiscountAmount = calculatingCartItemsResult.TotalDiscountAmount;
            var newOrder = new Order()
            {
                StoreId = loggedUser.StoreId,
                BranchId = loggedUser.BranchId,
                ShiftId = loggedUser.ShiftId,
                CustomerId = request.CustomerId,
                PromotionId = calculatingCartItemsResult?.DiscountTotalPromotion?.Id,
                AreaTableId = request.TableId,
                StatusId = EnumOrderStatus.Draft,
                OrderPaymentStatusId = null,
                OrderTypeId = request.EnumOrderTypeId,
                PaymentMethodId = request.EnumPaymentMethodId,
                OriginalPrice = totalOriginalPrice, /// The total original price items
                TotalDiscountAmount = totalDiscountAmount,
                PromotionDiscountValue = calculatingCartItemsResult.IsDiscountOnTotal ? totalDiscountAmount : 0, /// Promotion on total bill
                PromotionName = calculatingCartItemsResult?.DiscountTotalPromotion?.Name, /// Promotion on total bill
                IsPromotionDiscountPercentage = false,
                CashierName = shift.Staff.FullName,
                CustomerDiscountAmount = calculatingCartItemsResult.CustomerDiscountAmount,
                CustomerMemberShipLevel = calculatingCartItemsResult.CustomerMemberShipLevel,
                PlatformId = dataPlatformId,
                OrderItems = new List<OrderItem>(),
                OrderFees = new List<OrderFee>(),
                OrderSessions = new List<OrderSession>(),
            };

            if (loggedUser.StoreId.HasValue && loggedUser.StoreId != Guid.Empty)
            {
                var storeConfig = await _unitOfWork.StoreConfigs.GetStoreConfigByStoreIdAsync(loggedUser.StoreId.Value);
                newOrder.Code = storeConfig.CurrentMaxOrderCode.ConvertCodeFormat();
                await _unitOfWork.StoreConfigs.UpdateStoreConfigAsync(storeConfig, StoreConfigConstants.ORDER_CODE);
            }

            if (request.OrderStatus != EnumOrderStatus.Draft)
            {
                //Handle update order and payment status
                var orderStatus = await _orderService.GetOrderStatusAsync(newOrder.OrderTypeId, request.DeliveryMethod);
                newOrder.StatusId = orderStatus.OrderStatus;
                newOrder.OrderPaymentStatusId = orderStatus.PaymentStatus;

                if (request.IsDeliveryOrder)
                {
                    newOrder.OrderPaymentStatusId = request.Paid ? EnumOrderPaymentStatus.Paid : EnumOrderPaymentStatus.Unpaid;
                }
            }

            if (cartItems != null && cartItems.Any())
            {
                var productPriceIds = calculatingCartItemsResult.CartItems.Where(c => c.ProductPriceId.HasValue).Select(c => c.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => p.StoreId == loggedUser.StoreId && productPriceIds.Any(id => id == p.Id))
                    .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                    .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                    .Include(p => p.ProductPriceMaterials).ThenInclude(ppm => ppm.Material)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                var allOptionLevels = new List<OptionLevel>();
                var allOptionLevelIds = cartItems
                    .Where(i => i.IsCombo == true)
                    .SelectMany(i => i.Combo.ComboItems)
                    .SelectMany(c => c.Options.Select(o => o.OptionLevelId.Value));
                if (allOptionLevelIds.Any())
                {
                    allOptionLevels = await _unitOfWork.OptionLevels
                   .Find(o => o.StoreId == loggedUser.StoreId && allOptionLevelIds.Contains(o.Id))
                   .Include(o => o.Option)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken: cancellationToken);
                }

                var allToppingIds = cartItems
                    .Where(i => i.IsCombo == true)
                    .SelectMany(i => i.Combo.ComboItems)
                    .SelectMany(c => c.Toppings.Select(t => t.ToppingId));
                var allToppings = new List<Product>();
                if (allToppingIds.Any())
                {
                    allToppings = await _unitOfWork.Products
                    .Find(o => o.StoreId == loggedUser.StoreId && allToppingIds.Contains(o.Id))
                    .Include(t => t.ProductPrices)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);
                }

                var newOrderSession = CreateOrderSession(newOrder, loggedUser.AccountId.Value, loggedUser.StoreId.Value);

                /// Insert data to order item table
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.IsCombo)
                    {
                        var combo = cartItem.Combo;
                        var orderItem = CreateComboOrderItem(combo, newOrder.Id, newOrderSession.Id, allToppings, allOptionLevels, loggedUser.StoreId.Value);
                        newOrder.OrderItems.Add(orderItem);
                    }
                    else
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        var orderItem = CreateOrderItem(newOrder, productPrice, cartItem, newOrderSession.Id, loggedUser.StoreId.Value);
                        orderItem.OrderItemOptions = CreateOrderItemOptions(cartItem, orderItem, loggedUser.StoreId.Value);
                        orderItem.OrderItemToppings = CreateOrderItemToppings(cartItem, orderItem, loggedUser.StoreId.Value);
                        orderItem.ProductPriceName = productPrice.PriceName;

                        orderItem.PromotionId = cartItem.PromotionId;
                        orderItem.PromotionName = cartItem.PromotionName;
                        orderItem.PromotionDiscountValue = cartItem.PromotionValue;
                        orderItem.IsPromotionDiscountPercentage = cartItem.IsPercentDiscount;

                        newOrder.OrderItems.Add(orderItem);
                        newOrderSession.OrderItems.Add(orderItem);
                    }
                }

                /// Insert data to order fee table
                (var orderFees, var feeOrderSelects) = await CreateOrderFeesAsync(request, loggedUser.StoreId.Value, newOrder, cancellationToken); ;
                newOrder.OrderFees = orderFees;
                newOrder.TotalFee = _orderService.CalculateTotalFeeValue(totalOriginalPrice, feeOrderSelects);
                newOrder.TotalCost = await _orderService.CalculateTotalProductCostAsync(calculatingCartItemsResult.CartItems, productPrices);
                if (request.EnumOrderTypeId == EnumOrderType.Delivery || request.EnumOrderTypeId == EnumOrderType.Instore)
                {
                    newOrder.TotalTax = request.TotalTax;
                }


                newOrder.OrderSessions.Add(newOrderSession);

                await CreateOrderDeliveryAsync(newOrder, request, loggedUser, cartItems, calculatingCartItemsResult, cancellationToken);

                _unitOfWork.Orders.Add(newOrder);

                try
                {
                    await _unitOfWork.SaveChangesAsync();

                    await _orderService.SaveOrderHistoryAsync(newOrder.Id, null, newOrder.ToJsonWithCamelCase(), EnumOrderHistoryActionName.New.GetName(), EnumOrderHistoryActionName.New.GetNote(), null);

                    await _orderService.CloneOrderDetailAsync(newOrder, loggedUser.StoreId);

                    if (newOrder.StatusId != EnumOrderStatus.Draft)
                    {
                        await _orderService.CalMaterialQuantity(newOrder.Id, false, false, EnumInventoryHistoryAction.CreateOrder);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            #endregion

            await GetKitchenOrderSessionsAsync(loggedUser, cancellationToken);

            var response = Tuple.Create(newOrder.Id, newOrder.Code);

            return response;
        }

        private async Task<Tuple<Guid, string>> UpdateOrderAsync(
            GetProductCartItemResponse calculatingCartItemsResult,
            List<ProductCartItemModel> cartItems,// cart items split the quantity
            LoggedUserModel loggedUser,
            CreateOrderRequest request,
            CancellationToken cancellationToken,
            bool isPublicOrderDraft)
        {
            var shift = await _unitOfWork.Shifts
                .Find(s => s.StoreId == loggedUser.StoreId && s.Id == loggedUser.ShiftId)
                .Include(s => s.Staff)
                .Select(s => new
                {
                    s.Id,
                    s.Staff
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.BadRequestAgainstNull(shift, "Can not found shift information");

            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                             .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                             .Include(x => x.OrderFees)
                             .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                             .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);
            // draft order transfer into normal order
            if (order.StatusId == EnumOrderStatus.Draft && request.IsDraftOrderPublished)
            {
                order.StatusId = EnumOrderStatus.Processing;
                order.OrderPaymentStatusId = EnumOrderPaymentStatus.Unpaid;
            }

            var oldOrder = order.ToJsonWithCamelCase();

            using var updateOrderTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (order.StatusId != EnumOrderStatus.Draft)
                {
                    await _orderService.CalMaterialQuantity(request.OrderId.Value, true, true, EnumInventoryHistoryAction.EditOrder);
                }

                #region Handle update the order and create new related data to this order
                var totalOriginalPrice = calculatingCartItemsResult.OriginalPrice;
                var totalDiscountAmount = calculatingCartItemsResult.TotalDiscountAmount;
                order.StoreId = loggedUser.StoreId;
                order.BranchId = loggedUser.BranchId;
                order.ShiftId = loggedUser.ShiftId;
                order.CustomerId = request.CustomerId;
                order.PromotionId = calculatingCartItemsResult?.DiscountTotalPromotion?.Id;
                order.AreaTableId = request.TableId;
                order.OrderTypeId = request.EnumOrderTypeId;
                order.PaymentMethodId = request.EnumPaymentMethodId;
                order.OriginalPrice = totalOriginalPrice;
                order.TotalDiscountAmount = totalDiscountAmount;
                order.CashierName = shift.Staff.FullName;

                var productPriceIds = calculatingCartItemsResult.CartItems.Select(c => c.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => p.StoreId == loggedUser.StoreId && productPriceIds.Any(id => id == p.Id))
                    .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                    .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                    .Include(p => p.ProductPriceMaterials).ThenInclude(ppm => ppm.Material)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                var allToppingIds = cartItems
                    .Where(i => i.IsCombo == true)
                    .SelectMany(i => i.Combo.ComboItems)
                    .SelectMany(c => c.Toppings.Select(t => t.ToppingId));
                var allToppings = new List<Product>();
                if (allToppingIds.Any())
                {
                    allToppings = await _unitOfWork.Products
                    .Find(o => o.StoreId == loggedUser.StoreId && allToppingIds.Contains(o.Id))
                    .Include(t => t.ProductPrices)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);
                }

                var allOptionLevels = new List<OptionLevel>();
                var allOptionLevelIds = cartItems
                    .Where(i => i.IsCombo == true)
                    .SelectMany(i => i.Combo.ComboItems)
                    .SelectMany(c => c.Options.Select(o => o.OptionLevelId.Value));
                if (allOptionLevelIds.Any())
                {
                    allOptionLevels = await _unitOfWork.OptionLevels
                   .Find(o => o.StoreId == loggedUser.StoreId && allOptionLevelIds.Contains(o.Id))
                   .Include(o => o.Option)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken: cancellationToken);
                }

                var newOrderItems = new List<OrderItem>();
                var newOrderSession = CreateOrderSession(order, loggedUser.AccountId.Value, loggedUser.StoreId);

                /// add new items
                var newItems = cartItems.Where(item => item.OrderItemId == null || item.OrderItemId == Guid.Empty);
                foreach (var cartItem in newItems)
                {
                    if (cartItem.IsCombo)
                    {
                        var combo = cartItem.Combo;
                        var orderItem = CreateComboOrderItem(combo, order.Id, newOrderSession.Id, allToppings, allOptionLevels, loggedUser.StoreId);
                        newOrderItems.Add(orderItem);
                    }
                    else
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        if (productPrice == null)
                        {
                            continue;
                        }

                        var orderItem = CreateOrderItem(order, productPrice, cartItem, newOrderSession.Id, loggedUser.StoreId);
                        orderItem.OrderItemOptions = CreateOrderItemOptions(cartItem, orderItem, loggedUser.StoreId);
                        orderItem.OrderItemToppings = CreateOrderItemToppings(cartItem, orderItem, loggedUser.StoreId);
                        orderItem.ProductPriceName = productPrice.PriceName;
                        orderItem.PromotionId = cartItem.PromotionId;
                        orderItem.PromotionName = cartItem.PromotionName;
                        orderItem.PromotionDiscountValue = cartItem.PromotionValue;
                        orderItem.IsPromotionDiscountPercentage = cartItem.IsPercentDiscount;

                        newOrderItems.Add(orderItem);
                        newOrderSession.OrderItems.Add(orderItem);
                    }
                }

                /// update existed items
                var listExistedItemOrderItemId = cartItems.Select(x => x.OrderItemId);
                var cancelExistedItems = order.OrderItems.Where(item => !listExistedItemOrderItemId.Contains(item.Id));
                var orderItemWillBeUpdate = new List<OrderItem>();
                foreach (var cartItem in cancelExistedItems)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi => oi.Id == cartItem.Id);
                    if(orderItem != null)
                    {
                        orderItem.StatusId = EnumOrderItemStatus.Canceled;
                        orderItemWillBeUpdate.Add(orderItem);
                    }
                }

                /// Insert data to order fee table
                (var orderFees, var feeOrderSelects) = await CreateOrderFeesAsync(request, loggedUser.StoreId.Value, order, cancellationToken);
                _unitOfWork.OrderFees.RemoveRange(order.OrderFees);
                _unitOfWork.OrderFees.AddRange(orderFees);

                order.TotalFee = _orderService.CalculateTotalFeeValue(order.OriginalPrice, feeOrderSelects);
                order.TotalTax = request.TotalTax;

                /// Calculate order total cost
                order.TotalCost = await _orderService.CalculateTotalProductCostAsync(calculatingCartItemsResult.CartItems, productPrices);
                #endregion

                if (newItems != null && newItems.Any())
                {
                    _unitOfWork.OrderSessions.Add(newOrderSession);
                }

                _unitOfWork.OrderItems.UpdateRange(orderItemWillBeUpdate);
                _unitOfWork.OrderItems.AddRange(newOrderItems);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await updateOrderTransaction.CommitAsync(cancellationToken);

                await _orderService.CloneOrderDetailAsync(order, loggedUser.StoreId);
            }
            catch (Exception ex)
            {
                await updateOrderTransaction.RollbackAsync(cancellationToken);

                throw;
            }

            /// Save order history
            var newOrder = order.ToJsonWithCamelCase();
            await _orderService.SaveOrderHistoryAsync(order.Id, oldOrder, newOrder, EnumOrderHistoryActionName.Edit.GetName(), null, null);
            await GetKitchenOrderSessionsAsync(loggedUser, cancellationToken);

            if (order.StatusId != EnumOrderStatus.Draft)
            {
                await _orderService.CalMaterialQuantity(order.Id, false, false, EnumInventoryHistoryAction.EditOrder);
            }

            var result = Tuple.Create(order.Id, order.Code);

            return result;
        }

        private static OrderItem CreateOrderItem(Order order, ProductPrice productPrice, ProductCartItemModel cartItem, Guid orderSessionId, Guid? storeId)
        {
            var orderItem = new OrderItem()
            {
                OrderId = order.Id,
                ProductId = productPrice.ProductId,
                ProductName = productPrice?.Product?.Name,
                ProductPriceId = productPrice.Id,
                OriginalPrice = productPrice.PriceValue,
                PriceAfterDiscount = cartItem.PriceAfterDiscount,
                ProductPriceName = $"{cartItem.ItemName}",
                Quantity = cartItem.Quantity,
                Notes = cartItem.Notes,
                PromotionId = cartItem?.Promotion?.Id,
                PromotionName = cartItem?.Promotion?.Name,
                IsPromotionDiscountPercentage = cartItem?.Promotion?.IsPercentDiscount ?? false,
                PromotionDiscountValue = cartItem?.Promotion?.DiscountValue ?? 0,
                OrderItemOptions = new List<OrderItemOption>(),
                OrderItemToppings = new List<OrderItemTopping>(),
                OrderSessionId = orderSessionId,
                StatusId = EnumOrderItemStatus.New,
                StoreId = storeId
            };

            return orderItem;
        }

        private static List<OrderItemOption> CreateOrderItemOptions(ProductCartItemModel cartItem, OrderItem orderItem, Guid? storeId)
        {
            var orderItemOptions = new List<OrderItemOption>();
            if (cartItem.Options != null && cartItem.Options.Any())
            {
                foreach (var option in cartItem.Options)
                {
                    var orderItemOption = new OrderItemOption()
                    {
                        OrderItemId = orderItem.Id,
                        OptionLevelId = option.OptionLevelId,
                        OptionId = option.OptionId,
                        OptionName = option.OptionName,
                        OptionLevelName = option.OptionLevelName,
                        StoreId = storeId
                    };

                    orderItemOptions.Add(orderItemOption);
                }
            }

            return orderItemOptions;
        }

        private static List<OrderItemTopping> CreateOrderItemToppings(ProductCartItemModel cartItem, OrderItem orderItem, Guid? storeId)
        {
            var orderItemToppings = new List<OrderItemTopping>();
            if (cartItem.Toppings != null && cartItem.Toppings.Any())
            {
                foreach (var topping in cartItem.Toppings)
                {
                    var orderItemTopping = new OrderItemTopping()
                    {
                        OrderItemId = orderItem.Id,
                        ToppingId = topping.ToppingId,
                        PromotionId = topping.PromotionId,
                        ToppingName = topping.Name,
                        ToppingValue = topping.Price,
                        Quantity = topping.Quantity,
                        OriginalPrice = topping.OriginalPrice,
                        PriceAfterDiscount = topping.PriceAfterDiscount,
                        StoreId = storeId
                    };

                    orderItemToppings.Add(orderItemTopping);
                }
            }

            return orderItemToppings;
        }

        private async Task<(List<OrderFee>, List<Models.Fee.FeeModel>)> CreateOrderFeesAsync(CreateOrderRequest request, Guid storeId, Order order, CancellationToken cancellationToken)
        {
            var orderFees = new List<OrderFee>();
            var fees = new List<Models.Fee.FeeModel>();
            if (request.OrderFeeIds != null && request.OrderFeeIds.Any())
            {
                fees = await _unitOfWork.Fees
                    .GetFeesForCreateOrder(storeId, request.OrderFeeIds)
                    .AsNoTracking()
                    .ProjectTo<Models.Fee.FeeModel>(_mapperConfiguration)
                    .ToListAsync(cancellationToken: cancellationToken);

                foreach (var fee in fees)
                {
                    var orderFee = new OrderFee()
                    {
                        FeeId = fee.Id,
                        FeeName = fee.Name,
                        FeeValue = fee.Value,
                        IsPercentage = fee.IsPercentage,
                        OrderId = order.Id,
                        StoreId = storeId,
                    };

                    orderFees.Add(orderFee);
                }
            }

            return (orderFees, fees);
        }

        /// <summary>
        /// Create an order item with field IsCombo is true and create related OrderComboItem object
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="orderId"></param>
        /// <returns>OrderItem</returns>
        private static OrderItem CreateComboOrderItem(ComboOrderItemDto combo, Guid orderId, Guid orderSessionId, List<Product> allToppings, List<OptionLevel> allOptionLevels, Guid? storeId)
        {
            var comboOrderItem = new OrderItem()
            {
                OrderId = orderId,
                ProductPriceName = $"{combo.ItemName} {combo.ComboName}",
                OriginalPrice = combo.OriginalPrice,
                PriceAfterDiscount = combo.SellingPrice,
                Quantity = combo.Quantity,
                OrderSessionId = orderSessionId,
                StatusId = EnumOrderItemStatus.New,
                IsCombo = true,
                StoreId = storeId,
                OrderComboItem = new OrderComboItem()
                {
                    ComboId = combo.ComboId,
                    ComboPricingId = combo.ComboPricingId,
                    ComboName = combo.ComboName,
                    OriginalPrice = combo.OriginalPrice,
                    SellingPrice = combo.SellingPrice,
                    StoreId = storeId,
                    OrderComboProductPriceItems = new List<OrderComboProductPriceItem>()
                }
            };

            foreach (var comboItem in combo.ComboItems)
            {
                var orderComboProductPriceItem = new OrderComboProductPriceItem()
                {
                    OrderComboItemId = comboOrderItem.OrderComboItem.Id,
                    ProductPriceId = comboItem.ProductPriceId,
                    ItemName = comboItem.ItemName,
                    StatusId = EnumOrderItemStatus.New,
                    StoreId = storeId,
                    OrderItemOptions = new List<OrderItemOption>(),
                    OrderItemToppings = new List<OrderItemTopping>()
                };

                comboItem.Toppings.ForEach(requestTopping =>
                {
                    var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);

                    /// If the topping price is not found, continues
                    if (topping == null)
                    {
                        return;
                    }

                    /// The topping as a product and the product has multiple prices but the topping ONLY one price.
                    var toppingPrice = topping.ProductPrices.FirstOrDefault();
                    var toppingDto = new OrderItemTopping()
                    {
                        OrderItemId = comboOrderItem.Id,
                        ToppingId = topping.Id,
                        ToppingName = topping.Name,
                        ToppingValue = toppingPrice.PriceValue,
                        OriginalPrice = toppingPrice.PriceValue,
                        PriceAfterDiscount = toppingPrice.PriceValue,
                        Quantity = requestTopping.Quantity,
                        StoreId = storeId
                    };
                    orderComboProductPriceItem.OrderItemToppings.Add(toppingDto);
                });

                comboItem.Options.ForEach(requestOption =>
                {
                    var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == requestOption.OptionLevelId);

                    /// If the option level is not found, continues
                    if (optionLevel == null)
                    {
                        return;
                    }

                    var optionLevelDto = new OrderItemOption()
                    {
                        OptionId = optionLevel.OptionId,
                        OptionLevelId = optionLevel.Id,
                        OrderItemId = comboOrderItem.Id,
                        OptionName = optionLevel.Option.Name,
                        OptionLevelName = optionLevel.Name,
                        StoreId = storeId
                    };

                    orderComboProductPriceItem.OrderItemOptions.Add(optionLevelDto);
                });

                comboOrderItem.OrderComboItem.OrderComboProductPriceItems.Add(orderComboProductPriceItem);
            }

            var totalToppingPricePerItem = comboOrderItem.OrderComboItem.OrderComboProductPriceItems.SelectMany(c => c.OrderItemToppings).Sum(t => t.PriceAfterDiscount * t.Quantity);
            comboOrderItem.OriginalPrice = comboOrderItem.OrderComboItem.OriginalPrice + totalToppingPricePerItem;
            comboOrderItem.PriceAfterDiscount = comboOrderItem.OrderComboItem.SellingPrice + totalToppingPricePerItem;

            return comboOrderItem;
        }

        /// <summary>
        /// Split the item into pieces with the amount of 1
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        public async Task<List<ProductCartItemModel>> SplitOrderItems(List<ProductCartItemModel> cartItems, Guid? orderId, Guid? storeId)
        {
            var newCartItems = new List<ProductCartItemModel>();
            var oldOrderItems = new List<OrderItem>();
            if (orderId.HasValue)
            {
                oldOrderItems = await _unitOfWork.OrderItems
                .Find(oi => oi.StoreId == storeId && oi.OrderId == orderId)
                .Where(oi => oi.StatusId != EnumOrderItemStatus.Canceled)
                .OrderBy(oi => oi.CreatedTime)
                .Include(oi => oi.OrderComboItem).ThenInclude(oci => oci.OrderComboProductPriceItems).ThenInclude(ocppi => ocppi.OrderItemOptions)
                .Include(oi => oi.OrderComboItem).ThenInclude(oci => oci.OrderComboProductPriceItems).ThenInclude(ocppi => ocppi.OrderItemToppings)
                .Select(oi => new OrderItem()
                {
                    Id = oi.Id,
                    IsCombo = oi.IsCombo,
                    OrderComboItem = oi.OrderComboItem,
                    ProductPriceId = oi.ProductPriceId,
                    OrderItemOptions = oi.OrderItemOptions,
                    OrderItemToppings = oi.OrderItemToppings,
                })
                .AsNoTracking()
                .ToListAsync();
            }

            foreach (var cartItem in cartItems)
            {
                // new order
                if (orderId == null || orderId == Guid.Empty)
                {
                    lblQuantity: if (cartItem.Quantity >= 1)
                    {
                        cartItem.Quantity--;

                        if (cartItem.Combo != null)
                        {
                            cartItem.Combo.Quantity = 1;
                        }

                        var newCartItem = new ProductCartItemModel
                        {
                            OrderItemId = cartItem.OrderItemId,
                            ProductPriceId = cartItem.ProductPriceId,
                            OriginalPrice = cartItem.OriginalPrice,
                            PriceAfterDiscount = cartItem.PriceAfterDiscount,
                            Quantity = 1,
                            Notes = cartItem.Notes,
                            Options = cartItem.Options,
                            Toppings = cartItem.Toppings,
                            IsCombo = cartItem.IsCombo,
                            Combo = cartItem.Combo,
                            PromotionId = cartItem.PromotionId,
                            PromotionName = cartItem.PromotionName,
                            PromotionValue = cartItem.PromotionValue,
                            IsPercentDiscount = cartItem.IsPercentDiscount
                        };

                        newCartItems.Add(newCartItem);

                        goto lblQuantity;
                    }
                }
                //update order
                else
                {
                    if (cartItem.Combo != null)
                    {
                        cartItem.Combo.Quantity = 1;
                    }

                    if(cartItem.OrderItemId == null)
                    {
                        newItemQuantity: if (cartItem.Quantity >= 1)
                        {
                            cartItem.Quantity--;

                            var newCartItem = new ProductCartItemModel
                            {
                                OrderItemId = cartItem.OrderItemId,
                                ProductPriceId = cartItem.ProductPriceId,
                                OriginalPrice = cartItem.OriginalPrice,
                                PriceAfterDiscount = cartItem.PriceAfterDiscount,
                                Quantity = 1,
                                Notes = cartItem.Notes,
                                Options = cartItem.Options,
                                Toppings = cartItem.Toppings,
                                IsCombo = cartItem.IsCombo,
                                Combo = cartItem.Combo,
                                PromotionId = cartItem.PromotionId,
                                PromotionName = cartItem.PromotionName,
                                PromotionValue = cartItem.PromotionValue,
                                IsPercentDiscount = cartItem.IsPercentDiscount
                            };

                            newCartItems.Add(newCartItem);

                            goto newItemQuantity;
                        }

                        continue;
                    }

                    /// get all order items same existed order item, set new value from update request
                    var existedOrderItems = GetSameOrderItemsByOrderId(cartItem.OrderItemId.Value, oldOrderItems);
                    var numberUpdated = 0;
                    foreach (var orderItem in existedOrderItems)
                    {
                        if (numberUpdated >= cartItem.Quantity)
                        {
                            var newCartItem = new ProductCartItemModel
                            {
                                OrderItemId = orderItem.Id,
                                IsCanceled = true
                            };

                            newCartItems.Add(newCartItem);
                        }
                        else
                        {
                            var newCartItem = new ProductCartItemModel
                            {
                                OrderItemId = orderItem.Id,
                                ProductPriceId = cartItem.ProductPriceId,
                                OriginalPrice = cartItem.OriginalPrice,
                                PriceAfterDiscount = cartItem.PriceAfterDiscount,
                                Quantity = orderItem.Quantity,
                                Notes = cartItem.Notes,
                                Options = cartItem.Options,
                                Toppings = cartItem.Toppings,
                                IsCombo = cartItem.IsCombo,
                                Combo = cartItem.Combo,
                                PromotionId = cartItem.PromotionId,
                                PromotionName = cartItem.PromotionName,
                                PromotionValue = cartItem.PromotionValue,
                                IsPercentDiscount = cartItem.IsPercentDiscount
                            };

                            newCartItems.Add(newCartItem);
                            numberUpdated += 1;
                        }
                    }

                    /// split remain quantity to create new order item
                    var remainOrderItem = cartItem.Quantity - existedOrderItems.Count;
                    lblQuantity: if (remainOrderItem >= 1)
                    {
                        remainOrderItem--;
                        var newCartItem = new ProductCartItemModel
                        {
                            OrderItemId = null,
                            ProductPriceId = cartItem.ProductPriceId,
                            OriginalPrice = cartItem.OriginalPrice,
                            PriceAfterDiscount = cartItem.PriceAfterDiscount,
                            Quantity = 1,
                            Notes = cartItem.Notes,
                            Options = cartItem.Options,
                            Toppings = cartItem.Toppings,
                            IsCombo = cartItem.IsCombo,
                            Combo = cartItem.Combo,
                            PromotionId = cartItem.PromotionId,
                            PromotionName = cartItem.PromotionName,
                            PromotionValue = cartItem.PromotionValue,
                            IsPercentDiscount = cartItem.IsPercentDiscount
                        };

                        newCartItems.Add(newCartItem);

                        goto lblQuantity;
                    }
                }
            }

            return newCartItems;
        }

        private static List<OrderItem> GetSameOrderItemsByOrderId(Guid orderItemId, List<OrderItem> allOrderItems)
        {
            var orderItem = allOrderItems.FirstOrDefault(oi => oi.Id == orderItemId);
            var result = new List<OrderItem>();
            if (orderItem == null)
            {
                return result;
            }

            if (orderItem.IsCombo == true)
            {
                allOrderItems.ForEach(oi =>
                {
                    /// if item is product continue
                    if (oi.IsCombo == false)
                    {
                        return;
                    }

                    var isComboDuplicated = oi.OrderComboItem.ComboId == orderItem.OrderComboItem.ComboId;
                    var isComboItemsDuplicated = orderItem.OrderComboItem.OrderComboProductPriceItems.Count() == oi.OrderComboItem.OrderComboProductPriceItems.Count();
                    var comboItemsDuplicated = 0;
                    foreach (var existedComboItem in orderItem.OrderComboItem.OrderComboProductPriceItems)
                    {
                        foreach (var comboItemDuplicated in oi.OrderComboItem.OrderComboProductPriceItems)
                        {
                            var isOptionDuplicated = existedComboItem.OrderItemOptions.All(o => comboItemDuplicated.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                            var isToppingDuplicated = existedComboItem.OrderItemToppings.All(o => comboItemDuplicated.OrderItemToppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && existedComboItem.OrderItemToppings.Count() == comboItemDuplicated.OrderItemToppings.Count();
                            if (isOptionDuplicated && isToppingDuplicated)
                            {
                                comboItemsDuplicated += 1;
                                break;
                            }
                        }
                    }

                    if (isComboDuplicated && isComboItemsDuplicated && comboItemsDuplicated == orderItem.OrderComboItem.OrderComboProductPriceItems.Count())
                    {
                        result.Add(oi);
                    }
                });
            }
            else
            {
                allOrderItems.ForEach(oi =>
                {
                    /// if item is combo continue
                    if (oi.IsCombo == true)
                    {
                        return;
                    }

                    var isProductDuplicated = orderItem.ProductPriceId == oi.ProductPriceId && (orderItem.ProductPriceId != null || oi.ProductPriceId != null);
                    var isOptionDuplicated = orderItem.OrderItemOptions.All(o => oi.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                    var isToppingDuplicated = orderItem.OrderItemToppings.All(o => oi.OrderItemToppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && orderItem.OrderItemToppings.Count == oi.OrderItemToppings.Count;
                    if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
                    {
                        result.Add(oi);
                    }
                });
            }

            return result;
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

        private static OrderSession CreateOrderSession(Order order, Guid? accountId, Guid? storeId)
        {
            var orderSession = new OrderSession()
            {
                OrderId = order.Id,
                Order = order,
                StatusId = EnumOrderSessionStatus.New,
                CreatedUser = accountId,
                StoreId = storeId,
                OrderItems = new List<OrderItem>()
            };

            return orderSession;
        }

        /// <summary>
        /// Create order delivery
        /// </summary>
        /// <param name="order"></param>
        /// <param name="request"></param>
        /// <param name="loggedUser"></param>
        /// <param name="cartItems"></param>
        /// <param name="calculatingCartItemsResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CreateOrderDeliveryAsync(Order order, CreateOrderRequest request, LoggedUserModel loggedUser, List<ProductCartItemModel> cartItems,
           GetProductCartItemResponse calculatingCartItemsResult, CancellationToken cancellationToken)
        {
            if (request.IsDeliveryOrder)
            {
                var currentBranch = await _unitOfWork.StoreBranches.GetBranchAddressByStoreIdAndBranchIdAsync(loggedUser.StoreId, loggedUser.BranchId);
                var senderAddressDetail = FormatAddress(currentBranch.Address);
                order.DeliveryMethodId = request.DeliveryMethodId;
                order.DeliveryFee = request.DeliveryFee.Value;
                order.Note = request.Note;

                OrderDelivery orderDelivery = new()
                {
                    SenderAddress = senderAddressDetail,
                    SenderName = currentBranch.Name,
                    SenderPhone = currentBranch.PhoneNumber,
                    SenderLat = currentBranch.Address.Latitude,
                    SenderLng = currentBranch.Address.Longitude,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ReceiverAddress = request.ReceiverAddress.Address,
                    ReceiverLat = request.ReceiverAddress.Lat,
                    ReceiverLng = request.ReceiverAddress.Lng,
                    StoreId = loggedUser.StoreId
                };

                order.OrderDelivery = orderDelivery;

                var ahamoveConfig = await _unitOfWork.DeliveryConfigs
                    .Find(dc => dc.StoreId == loggedUser.StoreId && dc.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ahamoveConfig != null && request.DeliveryMethodId == ahamoveConfig.DeliveryMethodId)
                {
                    /// Calculate Ahamove COD price exclude shipping fee
                    var totalOriginalPrice = calculatingCartItemsResult.OriginalPrice;
                    var totalDiscountAmount = calculatingCartItemsResult.TotalDiscountAmount;
                    var totalFee = order.TotalFee;
                    var totalTax = order.TotalTax;
                    var deliveryFee = order.DeliveryFee;
                    var requestFee = totalOriginalPrice + totalFee + totalTax + deliveryFee - totalDiscountAmount;

                    var paymentMethod = EnumPaymentMethod.MoMo == request.EnumPaymentMethodId || EnumPaymentMethod.VNPay == request.EnumPaymentMethodId ? AhamoveDeliveryConfigConstants.PAYMENT_BALANCE : AhamoveDeliveryConfigConstants.PAYMENT_CASH;

                    var orderInfoAhamove = await CreateOrderAhaMoveAsync(ahamoveConfig, paymentMethod, currentBranch.Name, currentBranch.PhoneNumber, senderAddressDetail, currentBranch.Address.Latitude, currentBranch.Address.Longitude,
                         request.ReceiverName, request.ReceiverPhone, request.ReceiverAddress.Address, request.ReceiverAddress.Lat, request.ReceiverAddress.Lng, request.Note, requestFee,
                         loggedUser.StoreId, cartItems, calculatingCartItemsResult, cancellationToken);

                    if (orderInfoAhamove.OrderId != null)
                    {
                        order.AhamoveOrderId = orderInfoAhamove.OrderId;
                    }
                }
            }
        }

        /// <summary>
        /// Create order ahamove when choose ahamove delivery method
        /// </summary>
        ///  <param name="ahamoveConfig"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="senderName"></param>
        /// <param name="senderPhone"></param>
        /// <param name="senderAddress"></param>
        /// <param name="senderLatitude"></param>
        /// <param name="senderLongitude"></param>
        /// <param name="receiverName"></param>
        /// <param name="receiverPhone"></param>
        /// <param name="receiverAddress"></param>
        /// <param name="receiverLatitude"></param>
        /// <param name="receiverLongitude"></param>
        /// <param name="orderNote"></param>
        /// <param name="requestFee"></param>
        /// <param name="storeId"></param>
        /// <param name="cartItems"></param>
        /// <param name="calculatingCartItemsResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<CreateOrderAhamoveResponseModel> CreateOrderAhaMoveAsync(
           DeliveryConfig ahamoveConfig,
           string paymentMethod,
           string senderName,
           string senderPhone,
           string senderAddress,
           double? senderLatitude,
           double? senderLongitude,
           string receiverName,
           string receiverPhone,
           string receiverAddress,
           double receiverLatitude,
           double receiverLongitude,
           string orderNote,
           decimal? requestFee,
           Guid? storeId,
           List<ProductCartItemModel> cartItems,
           GetProductCartItemResponse calculatingCartItemsResult,
           CancellationToken cancellationToken
           )
        {
            var senderAddressDetail = new CreateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Name = senderName,
                Phone = senderPhone,
                Address = senderAddress,
                Lat = senderLatitude ?? 0,
                Lng = senderLongitude ?? 0,
            };

            var receiverAddressDetail = new CreateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Name = receiverName,
                Phone = receiverPhone,
                Address = receiverAddress,
                Lat = receiverLatitude,
                Lng = receiverLongitude,
                Remarks = orderNote,
                Cod = (double)requestFee,
            };

            var products = new List<CreateOrderAhamoveRequestModel.AhamoveProductDto>();
            if (cartItems != null && cartItems.Any())
            {
                var productPriceIds = calculatingCartItemsResult.CartItems.Where(c => c.ProductPriceId.HasValue).Select(c => c.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => p.StoreId == storeId && productPriceIds.Any(id => id == p.Id))
                    .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                    .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                    .Include(p => p.ProductPriceMaterials).ThenInclude(ppm => ppm.Material)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                foreach (var cartItem in cartItems)
                {
                    if (!cartItem.IsCombo)
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        var item = cartItems.FirstOrDefault(i => i.ProductPriceId == cartItem.ProductPriceId);
                        if (item == null || productPrice == null)
                        {
                            continue;
                        }

                        var product = new CreateOrderAhamoveRequestModel.AhamoveProductDto()
                        {
                            Id = productPrice.Product.Name,
                            Name = productPrice.Product.Name,
                            Amount = cartItem.Quantity,
                            Price = cartItem.PriceAfterDiscount,
                        };
                        products.Add(product);
                    }
                    else
                    {
                        var product = new CreateOrderAhamoveRequestModel.AhamoveProductDto()
                        {
                            Id = cartItem.Combo.ComboName,
                            Name = cartItem.Combo.ComboName,
                            Amount = cartItem.Combo.Quantity,
                            Price = cartItem.Combo.SellingPrice,
                        };
                        products.Add(product);
                    }
                }
            }

            var createOrderAhamoveRequest = new CreateOrderAhamoveRequestModel()
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
            var orderInfoAhamove = await _ahamoveService.CreateOrderAsync(infoToken.Token, createOrderAhamoveRequest);

            return orderInfoAhamove;
        }

        private static string FormatAddress(Address address)
        {
            var isDefaultCountry = address.Country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;

            List<string> addressComponents = new();
            if (address != null && !string.IsNullOrWhiteSpace(address.Address1))
            {
                addressComponents.Add(address.Address1);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.Address2) && !isDefaultCountry)
            {
                addressComponents.Add(address.Address2);
            }
            if (address != null && address.Ward != null && !string.IsNullOrWhiteSpace(address.Ward.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.Ward.Name);
            }
            if (address != null && address.District != null && !string.IsNullOrWhiteSpace(address.District.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.District.Name);
            }
            if (address != null && address.City != null && !string.IsNullOrWhiteSpace(address.City.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.City.Name);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.CityTown) && !isDefaultCountry)
            {
                addressComponents.Add(address.CityTown);
            }
            if (address != null && address.State != null && !string.IsNullOrWhiteSpace(address.State.Name) && !isDefaultCountry)
            {
                addressComponents.Add(address.State.Name);
            }
            if (!string.IsNullOrWhiteSpace(address.Country.Nicename))
            {
                addressComponents.Add(address.Country.Nicename);
            }

            return string.Join(", ", addressComponents);
        }

        private static List<ProductCartItemModel> MergeProductCartItems(List<ProductCartItemModel> productCartItems)
        {
            var result = new List<ProductCartItemModel>();
            foreach (var item in productCartItems)
            {
                if (item.IsCombo == true)
                {
                    var comboItemExisted = ComboItemDuplicated(item.Combo, result);
                    if (comboItemExisted == null)
                    {
                        result.Add(item);
                    }
                    else
                    {
                        comboItemExisted.Quantity += item.Quantity;
                        comboItemExisted.Combo.Quantity += item.Combo.Quantity;
                    }
                }
                else
                {
                    var existed = GetProductItemDuplicated(item, result);
                    if (existed == null)
                    {
                        result.Add(item);
                    }
                    else
                    {
                        existed.Quantity += item.Quantity;
                    }
                }
            }

            return result;
        }

        private static ProductCartItemModel GetProductItemDuplicated(ProductCartItemModel item, List<ProductCartItemModel> result)
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = item.ProductPriceId == existed.ProductPriceId && (item.ProductPriceId != null || existed.ProductPriceId != null);
                var isOptionDuplicated = item.Options.All(o => existed.Options.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.Toppings.All(o => existed.Toppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && item.Toppings.Count() == existed.Toppings.Count();
                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }

        private static ProductCartItemModel ComboItemDuplicated(ComboOrderItemDto item, List<ProductCartItemModel> result)
        {
            foreach (var existed in result)
            {
                var existedCombo = existed.Combo;
                if (existedCombo == null)
                {
                    continue;
                }

                var isComboDuplicated = (item.ComboId == existedCombo.ComboId);
                var comboItemsDuplicated = 0;
                foreach (var existedComboItem in existedCombo.ComboItems)
                {
                    var listComboItemDuplicated = item.ComboItems.Where(i => i.ProductPriceId == existedComboItem.ProductPriceId);
                    if (!listComboItemDuplicated.Any())
                    {
                        break;
                    };

                    foreach (var comboItemDuplicated in listComboItemDuplicated)
                    {
                        var isOptionDuplicated = existedComboItem.Options.All(o => comboItemDuplicated.Options.Any(e => e.OptionLevelId == o.OptionLevelId));
                        var isToppingDuplicated = existedComboItem.Toppings.All(o => comboItemDuplicated.Toppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && existedComboItem.Toppings.Count() == comboItemDuplicated.Toppings.Count();
                        if (isOptionDuplicated && isToppingDuplicated)
                        {
                            comboItemsDuplicated += 1;
                            break;
                        }
                    }
                }

                var isComboItemsDuplicated = comboItemsDuplicated == existedCombo.ComboItems.Count();
                if (isComboDuplicated && isComboItemsDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.POS.Application.Features.Products.Queries;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Fee;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetPrepareOrderEditDataRequest : IRequest<GetPrepareOrderEditDataResponse>
    {
        public Guid OrderId { get; set; }
    }

    public class GetPrepareOrderEditDataResponse : GetProductCartItemResponse
    {
        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public int CustomerPercent { get; set; }

        public string CustomerThumbnail { get; set; }

        public IEnumerable<Guid> OrderFeeIds { get; set; }

        public IEnumerable<FeeModel> Fees { get; set; }

        public AreaDto AreaTable { get; set; }

        public class AreaDto
        {
            public Guid? AreaId { get; set; }

            public Guid TableId { get; set; }

            public string AreaName { get; set; }

            public string TableName { get; set; }
        }

        public string OrderType { get; set; }

        public DateTime? OrderTime { get; set; }

        public int OrderTotalItems { get; set; }

        public string OrderCode { get; set; }

        public int OrderTypeId { get; set; }

        public EnumOrderStatus OrderStatusId { get; set; }

        public string OrderStatusName { get { return OrderStatusId.GetName(); } }

        public string OrderStatusColor { get { return OrderStatusId.GetColor(); } }

        public string OrderStatusBackgroundColor { get { return OrderStatusId.GetBackgroundColor(); } }

        public string PaymentMethod { get; set; }

        public int? OrderPaymentStatusId { get; set; }

        public string OrderPaymentStatus { get; set; }

        public string ReceiverInfo { get; set; }

        public string ReceiverAddress { get; set; }

        public string DeliveryMethod { get; set; }

        public decimal? DeliveryFee { get; set; }

        public decimal? PromotionDiscountValue { get; set; }

        public Guid PlatformId { get; set; }

        public string PlatformName { get; set; }
    }

    public class Handler : IRequestHandler<GetPrepareOrderEditDataRequest, GetPrepareOrderEditDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public Handler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            ICustomerService customerService,
            IOrderService orderService,
            IDateTimeService dateTimeService,
            IMapper mapper,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _customerService = customerService;
            _orderService = orderService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetPrepareOrderEditDataResponse> Handle(GetPrepareOrderEditDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                             .AsNoTracking()
                             .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderComboItem).ThenInclude(x => x.OrderComboProductPriceItems).ThenInclude(x => x.OrderItemOptions)
                             .Include(x => x.OrderItems).ThenInclude(i => i.OrderComboItem).ThenInclude(x => x.OrderComboProductPriceItems).ThenInclude(x => x.OrderItemToppings)
                             .Include(x => x.OrderFees).ThenInclude(o => o.Fee)
                             .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                             .Include(x => x.DeliveryMethod)
                             .Include(x => x.OrderDelivery)
                             .Include(x => x.Platform)
                             .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);

            var cartItems = await GetCartItemsFromOrderAsync(order, loggedUser.StoreId);

            /// Get table infomation
            var areaTable = new GetPrepareOrderEditDataResponse.AreaDto();
            if (order.AreaTable != null)
            {
                areaTable.AreaId = order?.AreaTable?.AreaId;
                areaTable.AreaName = order?.AreaTable?.Area?.Name;
                areaTable.TableId = order.AreaTable.Id;
                areaTable.TableName = order?.AreaTable?.Name;
            }

            var response = new GetPrepareOrderEditDataResponse()
            {
                CartItems = cartItems,
                IsDiscountOnTotal = false,
                OriginalPrice = order.OriginalPrice,
                TotalPriceAfterDiscount = order.PriceAfterDiscount,
                AreaTable = areaTable,
                CustomerDiscountAmount = order.CustomerDiscountAmount,
                CustomerId = order.CustomerId,
                DiscountTotalPromotion = new GetPrepareOrderEditDataResponse.PromotionDto(),
                PromotionDiscountValue = cartItems.Sum(x => x.Promotion == null ? 0 : x.Promotion.DiscountValue)
            };

            if (order.PromotionId.HasValue)
            {
                var promotion = await _unitOfWork.Promotions
                    .Find(p => p.StoreId == loggedUser.StoreId && p.Id == order.PromotionId.Value)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (promotion != null)
                {
                    var promotionDto = new GetPrepareOrderEditDataResponse.PromotionDto()
                    {
                        Id = promotion.Id,
                        Name = promotion.Name,
                        DiscountValue = order.PromotionDiscountValue
                    };

                    response.DiscountTotalPromotion = promotionDto;
                }
            }

            var allFeesInStore = await _unitOfWork.Fees
                .GetAllFeesInStore(loggedUser.StoreId.Value)
                .ProjectTo<FeeModel>(_mapperConfiguration)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var currentDate = _dateTimeService.NowUtc.Date;
            var feesActive = allFeesInStore.Where(fee => fee.StartDate == null
                                        && fee.EndDate != null
                                        && currentDate <= fee.EndDate.Value.Date
                                       || (fee.StartDate != null && fee.EndDate != null && fee.StartDate.Value.Date <= currentDate && fee.EndDate.Value.Date >= currentDate)
                                       || (fee.StartDate != null && fee.EndDate == null && fee.StartDate.Value.Date <= currentDate)
                                       || (fee.StartDate == null && fee.EndDate == null));

            response.Fees = feesActive;

            var fees = order.OrderFees.Select(o => o.Fee);
            response.TotalFee = _orderService.CalculateTotalFeeValue(order.OriginalPrice, fees);
            response.OrderFeeIds = fees.Select(f => f.Id);

            if (order.CustomerId.HasValue)
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdInStore(order.CustomerId.Value, loggedUser.StoreId);
                var membershipLevel = await _customerService.GetCustomerMembershipByCustomerIdAsync(customer.Id, loggedUser.StoreId.Value);
                response.CustomerName = customer?.FullName;
                response.CustomerPhone = customer?.PhoneNumber;
                response.CustomerMemberShipLevel = membershipLevel?.Name;
                response.CustomerPercent = membershipLevel.Discount;
                response.CustomerThumbnail = customer?.Thumbnail;
            }

            response.OrderType = order.OrderTypeId.GetName();
            response.OrderTypeId = (int)order.OrderTypeId;
            response.OrderCode = order.StringCode;
            response.OrderTime = order.CreatedTime;
            response.OrderTotalItems = cartItems.Count;
            response.OrderStatusId = order.StatusId;
            response.OrderPaymentStatusId = order.OrderPaymentStatusId.HasValue ? (int)order.OrderPaymentStatusId : null;
            response.PaymentMethod = order.PaymentMethodId.GetName();
            response.OrderPaymentStatus = order.OrderPaymentStatusId.HasValue ? order.OrderPaymentStatusId.Value.GetName() : string.Empty;
            if (response.OrderTypeId == (int)EnumOrderType.Delivery)
            {
                response.ReceiverInfo = $"{order?.OrderDelivery?.ReceiverName}-{order?.OrderDelivery?.ReceiverPhone}";
                response.ReceiverAddress = order?.OrderDelivery?.ReceiverAddress;
                response.DeliveryMethod = order?.DeliveryMethod?.Name;
                response.DeliveryFee = order?.DeliveryFee;
            }

            if (response.OrderTypeId == (int)EnumOrderType.Online && order != null)
            {
                response.DeliveryFee = order.DeliveryFee;
                response.TotalFee = order.TotalFee;
            }

            response.TotalTax = order.TotalTax;
            response.TotalDiscountAmount = order.TotalDiscountAmount;

            return response;
        }

        private async Task<List<ProductCartItemModel>> GetCartItemsFromOrderAsync(Order order, Guid? storeId)
        {
            var orderItems = order.OrderItems.Where(o => o.StatusId != EnumOrderItemStatus.Canceled);
            var optionLevelIdFromOrderItems = orderItems
               .Where(oi => oi.IsCombo == false)
               .SelectMany(oi => oi.OrderItemOptions.Where(oio => oio.OptionLevelId.HasValue).Select(oio => oio.OptionLevelId.Value));

            var optionLevelIdFromOrderComboItems = orderItems.Where(oi => oi.IsCombo == true)
                    .SelectMany(oi => oi.OrderComboItem.OrderComboProductPriceItems.SelectMany(ocppi =>
                                ocppi.OrderItemOptions.Where(oio => oio.OptionLevelId.HasValue).Select(oio => oio.OptionLevelId.Value)));

            var optionLevelIds = optionLevelIdFromOrderItems.Concat(optionLevelIdFromOrderComboItems).Distinct();
            var optionLevels = await _unitOfWork.OptionLevels
                .Find(o => o.StoreId == storeId && optionLevelIds.Contains(o.Id))
                .AsNoTracking()
                .Select(ol => new OptionLevel()
                {
                    Id = ol.Id,
                    IsSetDefault = ol.IsSetDefault
                })
                .ToListAsync();

            var cartItems = new List<ProductCartItemModel>();
            var orderItemList = order.OrderItems.Where(oi => oi.StatusId != EnumOrderItemStatus.Canceled).OrderBy(oi => oi.CreatedTime);
            foreach (var orderItem in orderItemList)
            {
                var cartItem = _mapper.Map<ProductCartItemModel>(orderItem);
                cartItem.Options = GetProductOptions(orderItem, optionLevels);
                cartItem.Toppings = _mapper.Map<IEnumerable<ProductCartItemModel.ToppingDto>>(orderItem.OrderItemToppings);

                #region Mapping options and toppings for combo
                if (orderItem.IsCombo)
                {
                    if (orderItem.OrderComboItem == null)
                    {
                        continue;
                    }

                    var comboItems = orderItem.OrderComboItem.OrderComboProductPriceItems.Select(orderComboItem => new ComboOrderItemDto.ComboItemDto()
                    {
                        ItemName = orderComboItem.ItemName,
                        Options = orderComboItem.OrderItemOptions.OrderBy(oi => oi.CreatedTime).Select(option => new ProductOptionDto
                        {
                            OptionId = option.OptionId,
                            OptionLevelId = option.OptionLevelId,
                            OptionLevelName = option.OptionLevelName,
                            OptionName = option.OptionName,
                            IsSetDefault = optionLevels.FirstOrDefault(ol => ol.Id == option.OptionLevelId)?.IsSetDefault ?? false
                        }).ToList(),
                        Toppings = orderComboItem.OrderItemToppings.Select(topping => new ComboOrderItemDto.ProductToppingDto
                        {
                            ToppingId = topping.ToppingId.Value,
                            Name = topping.ToppingName,
                            Quantity = topping.Quantity
                        }).ToList(),

                        ProductPriceId = orderComboItem.ProductPriceId
                    });

                    cartItem.Combo = _mapper.Map<ComboOrderItemDto>(orderItem.OrderComboItem);
                    cartItem.Combo.ComboItems = comboItems.ToList();
                    cartItem.ComboItems = comboItems;
                }
                #endregion

                cartItems.Add(cartItem);
            }

            /// TODO: Merge
            var result = MergeProductCartItems(cartItems);

            return result;
        }

        private static IEnumerable<ProductOptionDto> GetProductOptions(OrderItem orderItem, IEnumerable<OptionLevel> optionLevels)
        {
            var options = orderItem.OrderItemOptions
                .OrderBy(oi => oi.CreatedTime)
                .Select(itemOption => new ProductOptionDto()
                {
                    OptionId = itemOption.OptionId,
                    OptionLevelId = itemOption.OptionLevelId,
                    OptionLevelName = itemOption.OptionLevelName,
                    OptionName = itemOption.OptionName,
                    IsSetDefault = optionLevels.FirstOrDefault(ol => ol.Id == itemOption.OptionLevelId)?.IsSetDefault ?? false
                });

            return options;
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

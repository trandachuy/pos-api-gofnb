using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Fee;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetProductCartItemRequest : IRequest<GetProductCartItemResponse>
    {
        /// <summary>
        /// List of product cart items
        /// </summary>
        public List<OrderCartItemRequestModel> CartItems { get; set; }

        /// <summary>
        /// Applied fees
        /// </summary>
        public IEnumerable<Guid> OrderFeeIds { get; set; }

        /// <summary>
        /// Customer id to get discount
        /// </summary>
        public Guid? CustomerId { get; set; }

        public bool? SkipCheckOrderItems { get; set; }
    }

    public class GetProductCartItemResponse
    {
        public List<ProductCartItemModel> CartItems { get; set; }

        public bool IsDiscountOnTotal { get; set; } = false;

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; } = 0;

        public decimal TotalPriceAfterDiscount { get; set; }

        public PromotionDto DiscountTotalPromotion { get; set; }

        public class PromotionDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public decimal DiscountValue { get; set; }

            public decimal MaximumDiscountAmount { get; set; }
        }

        public decimal TotalFee { get; set; }

        public decimal TotalTax { get; set; }

        public decimal CustomerDiscountAmount { get; set; }

        public string CustomerMemberShipLevel { get; set; }

        public Guid? CustomerId { get; set; }

        public List<PromotionDiscountDto> Promotions { get; set; }

        public class PreparedOrderItemDto
        {
            public Guid OrderItemId { get; set; }

            public string ItemName { get; set; }
        }

        public class PromotionDiscountDto
        {
            public Guid Id { get; set; }

            public EnumPromotion PromotionType { get; set; }

            public string PromotionName { get; set; }

            public bool IsPercentDiscount { get; set; }

            public decimal MaximumDiscountAmount { get; set; }

            public bool? IsMinimumPurchaseAmount { get; set; }

            public decimal? MinimumPurchaseAmount { get; set; }

            public decimal TotalDiscountAmount { get; set; }

            /// <summary>
            /// <list>
            /// <item>
            /// This is product price ID if PromotionType is specific product
            /// </item>
            /// <item>
            /// This is product category ID if PromotionType is product category
            /// </item>
            /// <item>
            /// This is NULL: PromotionType total bill
            /// </item>
            /// </list>
            /// </summary>
            public Guid? TargetId { get; set; }
        }
    }

    public class GetProductCartItemRequestHandler : IRequestHandler<GetProductCartItemRequest, GetProductCartItemResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetProductCartItemRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IDateTimeService dateTimeService,
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _dateTimeService = dateTimeService;
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetProductCartItemResponse> Handle(GetProductCartItemRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var storeId = loggedUser.StoreId.Value;
            var branchId = loggedUser.BranchId.Value;

            ThrowError.Against(storeId == Guid.Empty || branchId == Guid.Empty, "Can not find store information.");

            var response = new GetProductCartItemResponse()
            {
                CartItems = new List<ProductCartItemModel>(),
                Promotions = new List<GetProductCartItemResponse.PromotionDiscountDto>()
            };

            if (!request.CartItems.Any())
            {
                await CalculateFeeAsync(request, response, storeId);
                response.TotalTax = 0;
                response.TotalFee = 0;
                response.TotalPriceAfterDiscount = 0;

                return response;
            };

            MergeProductCartItems(request);

            if (!request.SkipCheckOrderItems == true)
            {
                await CheckOrderItemStatusAsync(request, response, storeId);
            }

            var today = _dateTimeService.NowUtc;

            var promotionsInStore = await _unitOfWork.Promotions
                .GetAllPromotionInStore(storeId)
                .Where(p => p.IsStopped != true && (!p.EndDate.HasValue || p.EndDate.HasValue && p.EndDate.Value.Date.CompareTo(today.Date) >= 0))
                .AsNoTracking()
                .Include(p => p.PromotionProductCategories)
                .Include(p => p.PromotionProducts)
                .ToListAsync(cancellationToken: cancellationToken);

            /// Get all promotion activated today
            var promotionsInStoreActivated = promotionsInStore
                .Where(p =>
                p.EndDate.HasValue && today.Date.DateCompare(p.StartDate.Date) >= 0 && today.Date.DateCompare(p.EndDate.Value.Date) <= 0 ||
                today.Date.DateCompare(p.StartDate.Date) >= 0 &&
                (p.IsSpecificBranch == false || (p.IsSpecificBranch == true && p.PromotionBranches != null && p.PromotionBranches.Any(pb => pb.BranchId == branchId))));

            /// Find all DISCOUNT TOTAL BILL promotions for current branch
            var listPromotionTotalBill = promotionsInStoreActivated.Where(p => p.PromotionTypeId == (int)EnumPromotion.DiscountTotal);

            /// Find all DISCOUNT PRODUCT CATEGORY for current branch
            var listPromotionProductCategory = promotionsInStoreActivated.Where(p => p.PromotionTypeId == (int)EnumPromotion.DiscountProductCategory);

            /// Find all DISCOUNT PRODUCT for current branch
            var listPromotionProduct = promotionsInStoreActivated.Where(p => p.PromotionTypeId == (int)EnumPromotion.DiscountProduct);

            /// Get list product prices
            var productPriceIds = request.CartItems.Select(c => c.ProductPriceId);
            var productPrices = await _unitOfWork.ProductPrices
                .Find(p => p.StoreId == storeId && productPriceIds.Any(id => id == p.Id))
                .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                .Include(p => p.Product).ThenInclude(p => p.Tax)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            /// Get list option levels by all option levels in cart items
            var optionLevelIds = request.CartItems.SelectMany(c => c.Options.Select(o => o.OptionLevelId));
            var optionLevels = await _unitOfWork.OptionLevels.Find(ol => ol.StoreId == storeId && optionLevelIds.Any(olid => olid == ol.Id))
                .Include(ol => ol.Option)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            /// Get list toppings by all toppings in cart items
            var toppingIds = request.CartItems.SelectMany(c => c.Toppings.Select(t => t.ToppingId));
            var toppings = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(storeId)
                .AsNoTracking()
                .Where(t => toppingIds.Any(tid => tid == t.Id))
                .Include(t => t.ProductPrices)
                .ToListAsync(cancellationToken: cancellationToken);

            #region Mapping product price to cart item model

            var cartOrderItems = request.CartItems.Where(c => c.IsCombo == false);
            foreach (var cartItem in cartOrderItems)
            {
                var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                if (productPrice == null)
                {
                    continue;
                }

                var productCartItem = GetProductCartItemModel(productPrice, cartItem, toppings, optionLevels);
                response.CartItems.Add(productCartItem);
            }

            response.TotalTax = response.TotalTax + response.CartItems.Sum(x => x.ProductTax) ?? 0;
            #endregion

            response.OriginalPrice = CalculatingTotalOriginalPricelOnBill(response.CartItems);
            /// Find customer membership discount value
            if (request.CustomerId.HasValue)
            {
                var membershipLevel = await _customerService.GetCustomerMembershipByCustomerIdAsync(request.CustomerId.Value, storeId);
                var membershipDiscountValue = await _customerService.GetCustomerMembershipDiscountValueByCustomerIdAsync(response.OriginalPrice, request.CustomerId.Value, storeId);
                response.CustomerDiscountAmount = membershipDiscountValue;
                response.CustomerMemberShipLevel = membershipLevel?.Name;
                response.CustomerId = request.CustomerId;
            }

            #region Apply promotion
            /// Promotion discount on total bill
            var (totalBillPromotion, totalBillDiscountValue) = FindMaxPromotion(listPromotionTotalBill, response.OriginalPrice);
            if (listPromotionTotalBill != null && listPromotionTotalBill.Any() && totalBillPromotion != null && totalBillDiscountValue > 0)
            {
                response.IsDiscountOnTotal = true;
                response.TotalDiscountAmount += totalBillDiscountValue;
                CollectPromotionApplied(response, totalBillPromotion, totalBillDiscountValue);
            }
            else
            {
                /// Promotion discount on product category
                var listItemIdHadDiscount = new List<Guid>();
                if (listPromotionProductCategory != null && listPromotionProductCategory.Any())
                {
                    decimal totalDiscountByProductCategoryPromotion = 0;
                    var promotionDiscounts = new List<Tuple<Guid, Promotion, decimal>>();

                    var listProductCategoryIdsByPromotion = listPromotionProductCategory
                        .SelectMany(p => p.PromotionProductCategories)
                        .Select(p => p.ProductCategoryId)
                        .Distinct();

                    var listOrderItemHasPromotionByProductCategory = response.CartItems.Where(i => listProductCategoryIdsByPromotion.Contains(i.ProductCategoryId.Value));
                    foreach (var orderItem in listOrderItemHasPromotionByProductCategory)
                    {
                        Promotion promotion = new();
                        decimal discountValue = 0;
                        var existedPromotionDiscount = promotionDiscounts.FirstOrDefault(i => i.Item1 == orderItem.ProductCategoryId);
                        if (existedPromotionDiscount != null &&
                            existedPromotionDiscount.Item2.MaximumDiscountAmount > 0 &&
                            existedPromotionDiscount.Item3 <= existedPromotionDiscount.Item2.MaximumDiscountAmount)
                        {
                            var remainDiscountAmount = existedPromotionDiscount.Item2.MaximumDiscountAmount - existedPromotionDiscount.Item3;
                            discountValue = CalculateDiscount(orderItem.OriginalPrice, existedPromotionDiscount.Item2);

                            if (discountValue >= remainDiscountAmount)
                            {
                                discountValue = remainDiscountAmount;
                            }

                            promotion = existedPromotionDiscount.Item2;
                            decimal currentDiscountValue = existedPromotionDiscount.Item3 + discountValue;
                            promotionDiscounts.Remove(existedPromotionDiscount);
                            promotionDiscounts.Add(new Tuple<Guid, Promotion, decimal>(orderItem.ProductCategoryId.Value, promotion, currentDiscountValue));

                        }
                        else
                        {
                            (promotion, discountValue) = FindMaxPromotion(listPromotionProductCategory, orderItem.OriginalPrice * orderItem.Quantity);
                            promotionDiscounts.Add(new Tuple<Guid, Promotion, decimal>(orderItem.ProductCategoryId.Value, promotion, discountValue));
                        }

                        var orderItemDiscountValuePerUnit = CalculateDiscount(orderItem.OriginalPrice * orderItem.Quantity, promotion, existedPromotionDiscount?.Item3 ?? null);
                        orderItem.PriceAfterDiscount = orderItem.OriginalPrice - (orderItemDiscountValuePerUnit / orderItem.Quantity);

                        orderItem.TotalPriceAfterDiscount = orderItem.OriginalPrice * orderItem.Quantity - discountValue;
                        orderItem.TotalDiscount += discountValue;

                        totalDiscountByProductCategoryPromotion += discountValue;
                        listItemIdHadDiscount.Add(orderItem.ProductPriceId.Value);

                        CollectPromotionApplied(response, promotion, discountValue, orderItem);
                    }

                    response.TotalDiscountAmount += totalDiscountByProductCategoryPromotion;
                }

                /// Promotion discount on product - only apply for item has not been promotion applied yet
                if (listPromotionProduct != null && listPromotionProduct.Any())
                {
                    decimal totalDiscountBySpecificProductPromotion = 0;
                    var listProductIdsHasPromotion = listPromotionProduct
                        .SelectMany(p => p.PromotionProducts)
                        .Select(p => p.ProductId)
                        .Distinct();

                    var listItemHasPromotion = response.CartItems.Where(i => i.ProductId.HasValue && listProductIdsHasPromotion.Contains(i.ProductId.Value));
                    foreach (var orderItem in response.CartItems)
                    {
                        var listPromotionForProductItem = listPromotionProduct
                            .Where(p => orderItem.ProductId.HasValue &&
                                        p.PromotionProducts.Select(x => x.ProductId).Contains(orderItem.ProductId.Value));

                        if (listItemIdHadDiscount.Any(oiid => oiid == orderItem.ProductPriceId))
                        {
                            continue;
                        }

                        if (!listProductIdsHasPromotion.Any(pid => pid == orderItem.ProductId))
                        {
                            continue;
                        }

                        var (promotion, discountValue) = FindMaxPromotion(listPromotionForProductItem, orderItem.OriginalPrice * orderItem.Quantity);
                        var orderItemDiscountValuePerUnit = CalculateDiscount(orderItem.OriginalPrice * orderItem.Quantity, promotion);
                        orderItem.PriceAfterDiscount = orderItem.OriginalPrice - (orderItemDiscountValuePerUnit / orderItem.Quantity);
                        orderItem.TotalPriceAfterDiscount = orderItem.OriginalPrice * orderItem.Quantity - discountValue;
                        orderItem.TotalDiscount += discountValue;
                        totalDiscountBySpecificProductPromotion += discountValue;

                        CollectPromotionApplied(response, promotion, discountValue, orderItem);
                    }

                    response.TotalDiscountAmount += totalDiscountBySpecificProductPromotion;
                }
            }

            #endregion

            await ComboHandlerAsync(request, response, storeId);

            await CalculateFeeAsync(request, response, storeId);

            response.TotalPriceAfterDiscount = response.OriginalPrice - response.TotalDiscountAmount;

            return response;
        }

        private static void CollectPromotionApplied(GetProductCartItemResponse response, Promotion promotion, decimal discountValue, ProductCartItemModel cartItem = null)
        {
            if (0 < discountValue && discountValue <= promotion.MaximumDiscountAmount)
            {
                response.Promotions.Add(new GetProductCartItemResponse.PromotionDiscountDto()
                {
                    Id = promotion.Id,
                    PromotionName = promotion.Name,
                    PromotionType = (EnumPromotion)promotion.PromotionTypeId,
                    IsMinimumPurchaseAmount = promotion.IsMinimumPurchaseAmount,
                    IsPercentDiscount = promotion.IsPercentDiscount,
                    MaximumDiscountAmount = promotion.MaximumDiscountAmount,
                    MinimumPurchaseAmount = promotion.MinimumPurchaseAmount,
                    TotalDiscountAmount = discountValue
                });

                // Map Promotion Information for cart Item to serve the Create Order
                if (cartItem != null)
                {
                    cartItem.PromotionId = promotion.Id;
                    cartItem.PromotionName = promotion.Name;
                    cartItem.PromotionValue = discountValue;
                    cartItem.IsPercentDiscount = promotion.IsPercentDiscount;
                }
            }
        }

        /// <summary>
        /// Find max promotion from promotions for the price
        /// </summary>
        /// <param name="promotions"></param>
        /// <param name="price"></param>
        /// <returns>Promotion, Discount value</returns>
        private static (Promotion, decimal) FindMaxPromotion(IEnumerable<Promotion> promotions, decimal price)
        {
            var maxPromotion = new Promotion();
            decimal discountValue = 0;
            foreach (var promotion in promotions)
            {
                var minimumRequired = promotion.IsMinimumPurchaseAmount == true ? promotion.MinimumPurchaseAmount : 0;
                var maxDiscount = CalculateDiscount(price, promotion);
                if (maxDiscount >= discountValue && price >= minimumRequired)
                {
                    discountValue = maxDiscount;
                    maxPromotion = promotion;
                }
            }

            return (maxPromotion, discountValue);
        }

        private ProductCartItemModel GetProductCartItemModel(
            ProductPrice productPrice,
            OrderCartItemRequestModel cartItem,
            List<Product> toppings,
            List<OptionLevel> optionLevels)
        {
            var productCategory = productPrice.Product.ProductProductCategories.FirstOrDefault();
            var taxValue = productPrice?.Product?.Tax?.Percentage == null ? 0 : (productPrice?.Product?.Tax?.Percentage * productPrice.PriceValue * cartItem.Quantity) / 100;
            var productCartItem = new ProductCartItemModel()
            {
                OrderItemId = cartItem.OrderItemId,
                ProductPriceId = productPrice.Id,
                ItemName = _productService.BuildProductName(productPrice.Product.Name, productPrice.PriceName),
                OriginalPrice = productPrice.PriceValue,
                PriceAfterDiscount = productPrice.PriceValue,
                TotalDiscount = 0,
                TotalPriceAfterDiscount = productPrice.PriceValue * cartItem.Quantity,
                Quantity = cartItem.Quantity,
                ProductPriceName = productPrice.PriceName,
                ProductCategoryId = productCategory?.ProductCategoryId,
                ProductId = cartItem.ProductId,
                OrderId = cartItem.OrderId,
                Thumbnail = productPrice?.Product?.Thumbnail,
                ProductTax = taxValue
            };

            var itemToppings = GetToppingsDataFromRequest(toppings, cartItem.Toppings);
            var itemOptions = GetOptionsDataFromRequest(optionLevels, cartItem.Options);

            productCartItem.Toppings = itemToppings;
            productCartItem.Options = itemOptions;

            return productCartItem;
        }

        private static List<ProductCartItemModel.ToppingDto> GetToppingsDataFromRequest(List<Product> allToppings, IEnumerable<ProductToppingModel> requestToppings)
        {
            var result = new List<ProductCartItemModel.ToppingDto>();
            foreach (var requestTopping in requestToppings)
            {
                var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);
                if (topping == null)
                {
                    continue;
                }

                var toppingPrice = topping.ProductPrices.FirstOrDefault();
                var totalToppingPricePerItem = (toppingPrice?.PriceValue ?? 0) * requestTopping.Quantity;
                var toppingDto = new ProductCartItemModel.ToppingDto()
                {
                    ToppingId = topping.Id,
                    Name = topping.Name,
                    Quantity = requestTopping.Quantity,
                    OriginalPrice = totalToppingPricePerItem, /// Total topping price per cart item
                    PriceAfterDiscount = totalToppingPricePerItem /// Total topping price per cart item after apply promotion
                };

                result.Add(toppingDto);
            }

            return result;
        }

        /// <summary>
        /// Mapping optionLevel to ProductOptionDto
        /// </summary>
        /// <param name="allOptionLevels"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        private static List<ProductOptionDto> GetOptionsDataFromRequest(List<OptionLevel> allOptionLevels, IEnumerable<ProductOptionDto> requestOptions)
        {
            var result = new List<ProductOptionDto>();
            foreach (var requestOption in requestOptions)
            {
                var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == requestOption.OptionLevelId);
                if (optionLevel == null)
                {
                    continue;
                }

                var optionLevelDto = new ProductOptionDto()
                {
                    OptionId = optionLevel.OptionId,
                    OptionLevelId = optionLevel.Id,
                    OptionName = optionLevel.Option.Name,
                    OptionLevelName = optionLevel.Name,
                    IsSetDefault = optionLevel.IsSetDefault ?? false
                };

                result.Add(optionLevelDto);
            }

            return result;
        }

        /// <summary>
        /// If promotion is discount by percentage, calculate discount value but maximum value is MaximumDiscountAmount of promotion.
        /// If promotion is not discount by percentage, the discount value is MaximumDiscountAmount of promotion.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="promotion"></param>
        /// <returns>Maximum discount value</returns>
        private static decimal CalculateDiscount(decimal price, Promotion promotion, decimal? currentDiscountValue = null)
        {
            if (promotion.IsPercentDiscount)
            {
                var discountValue = price * (decimal)promotion.PercentNumber / 100;

                if (currentDiscountValue.HasValue)
                {
                    if (currentDiscountValue == promotion.MaximumDiscountAmount)
                    {
                        return 0;
                    }

                    return discountValue >= promotion.MaximumDiscountAmount ?
                                promotion.MaximumDiscountAmount - currentDiscountValue.Value :
                                discountValue - currentDiscountValue.Value;
                }
                if (promotion.MaximumDiscountAmount > 0)
                {
                    return discountValue >= promotion.MaximumDiscountAmount ? promotion.MaximumDiscountAmount : discountValue;
                }

                return discountValue;
            }
            else
            {
                if (currentDiscountValue.HasValue && currentDiscountValue <= promotion.MaximumDiscountAmount)
                {
                    return promotion.MaximumDiscountAmount - currentDiscountValue.Value;
                }

                return promotion.MaximumDiscountAmount;
            }
        }

        private static decimal CalculatingPriceAfterDiscount(decimal price, decimal discountValue)
        {
            var priceAfterDiscount = price - discountValue;

            return priceAfterDiscount <= 0 ? 0 : priceAfterDiscount;
        }

        private static decimal CalculatingTotalOriginalPricelOnBill(List<ProductCartItemModel> cartItems)
        {
            decimal totalItemOriginalPrice = cartItems.Sum(i => i.OriginalPrice * i.Quantity);
            decimal totalPriceTopping = 0;
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Toppings == null || !cartItem.Toppings.Any())
                {
                    continue;
                }

                var priceOfToppings = cartItem.Toppings.Sum(t => t.OriginalPrice);
                totalPriceTopping += (priceOfToppings * cartItem.Quantity);
            }

            decimal result = totalItemOriginalPrice + totalPriceTopping;

            return result;
        }

        /// <summary>
        /// Calculate total fee value by fee ids
        /// </summary>
        /// <param name="originalPrice"></param>
        /// <param name="feeIds"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private async Task<decimal> CalculateTotalFeeAsync(decimal originalPrice, IEnumerable<Guid> feeIds, Guid storeId)
        {
            decimal totalFeeValue = 0;
            if (feeIds != null && feeIds.Any())
            {
                var fees = await _unitOfWork.Fees
                  .GetAllFeesInStore(storeId)
                  .Where(f => feeIds.Any(feeId => feeId == f.Id))
                  .AsNoTracking()
                  .ProjectTo<FeeModel>(_mapperConfiguration)
                  .ToListAsync();

                if (fees.Count > 0)
                {
                    totalFeeValue = _orderService.CalculateTotalFeeValue(originalPrice, fees);
                }
            }

            return totalFeeValue;
        }

        private async Task CalculateFeeAsync(GetProductCartItemRequest request, GetProductCartItemResponse response, Guid storeId)
        {
            #region Handle calculate OriginalPrice and TotalPriceAfterDiscount when combo included
            var combos = response.CartItems.Where(c => c.IsCombo == true).Select(c => c.Combo);
            if (response.CartItems.Any() && combos.Any())
            {
                decimal totalComboSellingPriceValue = 0;
                decimal totalComboOriginalPriceValue = 0;
                foreach (var combo in combos)
                {
                    var toppings = combo.ComboItems.SelectMany(i => i.Toppings);
                    var toppingPriceValue = toppings.Sum(t => t.PriceAfterDiscount * t.Quantity);

                    var comboSellingPriceValue = (combo.SellingPrice + toppingPriceValue) * combo.Quantity;
                    totalComboSellingPriceValue += comboSellingPriceValue;

                    decimal comboOriginalPriceValue = (combo.OriginalPrice + toppingPriceValue) * combo.Quantity;
                    totalComboOriginalPriceValue += comboOriginalPriceValue;
                }

                response.OriginalPrice += totalComboOriginalPriceValue;
                response.TotalPriceAfterDiscount += totalComboSellingPriceValue;
            }
            #endregion

            response.TotalFee = await CalculateTotalFeeAsync(response.OriginalPrice, request.OrderFeeIds, storeId);
            if (response.TotalFee > 0)
            {
                response.TotalPriceAfterDiscount += response.TotalFee;
            }
        }

        private async Task ComboHandlerAsync(
            GetProductCartItemRequest request,
            GetProductCartItemResponse response,
            Guid storeId)
        {
            var cartItems = request.CartItems
                .Where(c => c.IsCombo == true && c.Combo.ComboItems != null)
                .Select(c => new
                {
                    c.OrderItemId,
                    c.Combo
                });

            var productPrices = await _unitOfWork.ProductPrices
                .GetAllProductPriceInStore(storeId)
                .AsNoTracking()
                .ToListAsync();

            var allOptionLevelIds = cartItems.Select(i => i.Combo).SelectMany(c => c.ComboItems.SelectMany(i => i.Options.Select(o => o.OptionLevelId)));

            var allOptionLevels = await _unitOfWork.OptionLevels
                .Find(o => o.StoreId == storeId && allOptionLevelIds.Any(oid => oid == o.Id))
                .AsNoTracking()
                .Include(o => o.Option)
                .ToListAsync();

            var allToppingIds = cartItems.Select(i => i.Combo).SelectMany(c => c.ComboItems.SelectMany(i => i.Toppings.Select(o => o.ToppingId)));
            var allToppings = await _unitOfWork.Products
                .Find(o => o.StoreId == storeId && allToppingIds.Any(oid => oid == o.Id))
                .AsNoTracking()
                .Include(t => t.ProductPrices)
                .ToListAsync();

            var comboImages = await _unitOfWork.Combos
                .GetAllCombosInStore(storeId)
                .Where(c => cartItems.Select(i => i.Combo.ComboId).Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    c.Thumbnail
                })
                .ToListAsync();

            decimal totalOriginalPrice = 0;
            decimal totalSellingPrice = 0;
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Combo.ComboItems == null)
                {
                    continue;
                }

                var combo = cartItem.Combo;
                var comboImage = comboImages.FirstOrDefault(c => c.Id == combo.ComboId);
                var comboDto = new ComboOrderItemDto()
                {
                    ComboId = combo.ComboId,
                    ComboPricingId = combo.ComboPricingId,
                    ComboName = combo.ComboName,
                    ItemName = combo.ItemName,
                    OriginalPrice = combo.OriginalPrice,
                    SellingPrice = combo.SellingPrice,
                    Quantity = combo.Quantity,
                    Thumbnail = comboImage?.Thumbnail,
                    ComboItems = new List<ComboOrderItemDto.ComboItemDto>(),
                };

                totalOriginalPrice += combo.OriginalPrice * combo.Quantity;
                totalSellingPrice += combo.SellingPrice * combo.Quantity;

                foreach (var item in combo.ComboItems)
                {
                    var productPrice = productPrices.FirstOrDefault(p => p.Id == item.ProductPriceId);
                    if (productPrice == null)
                    {
                        continue;
                    }

                    var newComboItem = new ComboOrderItemDto.ComboItemDto()
                    {
                        ProductPriceId = item.ProductPriceId,
                        ItemName = _productService.BuildProductName(productPrice.Product.Name, productPrice.PriceName),
                        Quantity = item.Quantity > 0 ? item.Quantity : 1,
                        Toppings = new List<ComboOrderItemDto.ProductToppingDto>(),
                        Options = new List<ProductOptionDto>(),
                        ProductId = item.ProductId,
                        Thumbnail = productPrice?.Product?.Thumbnail,
                    };

                    var comboTax = productPrice?.Product?.Tax?.Percentage == null ? 0 : (productPrice?.Product?.Tax?.Percentage * newComboItem.Quantity * productPrice.PriceValue) / 100;
                    response.TotalTax = response.TotalTax + comboTax ?? 0;

                    item.Toppings.ForEach(requestTopping =>
                    {
                        var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);

                        /// If the topping price is not found, continues
                        if (topping == null)
                        {
                            return;
                        }

                        /// The topping as a product and the product has multiple prices but the topping ONLY one price.
                        var toppingPrice = topping.ProductPrices.FirstOrDefault()?.PriceValue ?? 0;

                        /// The topping price in combo will be not apply promotion.
                        var toppingDto = new ComboOrderItemDto.ProductToppingDto()
                        {
                            ToppingId = topping.Id,
                            Name = topping.Name,
                            Quantity = requestTopping.Quantity,
                            OriginalPrice = toppingPrice,
                            PriceAfterDiscount = toppingPrice
                        };

                        newComboItem.Toppings.Add(toppingDto);

                        totalOriginalPrice += toppingDto.OriginalPrice * requestTopping.Quantity * combo.Quantity;
                        totalSellingPrice += toppingDto.PriceAfterDiscount * requestTopping.Quantity * combo.Quantity;
                    });

                    item.Options.ForEach(requestOption =>
                    {
                        var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == requestOption.OptionLevelId);

                        /// If the option level is not found, continues
                        if (optionLevel == null)
                        {
                            return;
                        }

                        var optionLevelDto = new ProductOptionDto()
                        {
                            OptionId = optionLevel.OptionId,
                            OptionLevelId = optionLevel.Id,
                            OptionName = optionLevel.Option.Name,
                            OptionLevelName = optionLevel.Name,
                            IsSetDefault = optionLevel.IsSetDefault ?? false
                        };

                        newComboItem.Options.Add(optionLevelDto);
                    });

                    comboDto.ComboItems.Add(newComboItem);
                }

                var newProductCartItem = new ProductCartItemModel()
                {
                    OrderItemId = cartItem.OrderItemId,
                    IsCombo = true,
                    Combo = comboDto,
                    Quantity = combo.Quantity
                };

                response.CartItems.Add(newProductCartItem);
            }

            var comboDiscount = totalOriginalPrice - totalSellingPrice;
            response.TotalDiscountAmount += comboDiscount;
        }

        /// <summary>
        /// Group same product item and quantity
        /// </summary>
        /// <param name="request"></param>
        private static void MergeProductCartItems(GetProductCartItemRequest request)
        {
            var result = new List<OrderCartItemRequestModel>();
            foreach (var item in request.CartItems)
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
                    /// Product duplicated
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

            request.CartItems = result;
        }

        private static OrderCartItemRequestModel GetProductItemDuplicated(OrderCartItemRequestModel item, List<OrderCartItemRequestModel> result)
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = (item.ProductPriceId == existed.ProductPriceId) && item.ProductPriceId != Guid.Empty;
                var isOptionDuplicated = item.Options.All(o => existed.Options.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.Toppings.All(o => existed.Toppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && item.Toppings.Count == existed.Toppings.Count;

                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }

        private static OrderCartItemRequestModel ComboItemDuplicated(ComboOrderItemDto item, List<OrderCartItemRequestModel> result)
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
                        var isToppingDuplicated = existedComboItem.Toppings.All(o => comboItemDuplicated.Toppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && existedComboItem.Toppings.Count == comboItemDuplicated.Toppings.Count;
                        if (isOptionDuplicated && isToppingDuplicated)
                        {
                            comboItemsDuplicated += 1;
                            break;
                        }
                    }
                }

                var isComboItemsDuplicated = comboItemsDuplicated == existedCombo.ComboItems.Count;
                if (isComboDuplicated && isComboItemsDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }

        /// <summary>
        /// Check order item status from kitchen
        /// </summary>
        /// <param name="response"></param>
        /// <param name="listOrderItemIdChanged"></param>
        /// <returns></returns>
        private async Task CheckOrderItemStatusAsync(GetProductCartItemRequest request, GetProductCartItemResponse response, Guid? storeId)
        {
            var orderItemId = request.CartItems.Where(c => c.OrderItemId != null).Select(c => c.OrderItemId.Value).FirstOrDefault();
            if (orderItemId == Guid.Empty)
            {
                return;
            }

            var firstOrderItem = await _unitOfWork.OrderItems.Find(oi => oi.StoreId == storeId && oi.Id == orderItemId).FirstOrDefaultAsync();
            var orderItems = await _unitOfWork.OrderItems
                .Find(oi => oi.StoreId == storeId && oi.OrderId == firstOrderItem.OrderId)
                .Include(oi => oi.OrderItemOptions)
                .Include(oi => oi.OrderItemToppings)
                .AsNoTracking()
                .ToListAsync();

            var mergedSameOrderItems = _orderService.MergeSameOrderItems(orderItems);

            var preparedOrderItems = new List<GetProductCartItemResponse.PreparedOrderItemDto>();
            foreach (var orderItem in mergedSameOrderItems)
            {
                var requestItem = request.CartItems.FirstOrDefault(c => c.OrderItemId == orderItem.Item1.Id);
                if (requestItem == null)
                {
                    continue;
                }

                var orderedItemQuantity = orderItem.Item2.Count;
                var currentItemQuantity = requestItem.Quantity;
                var orderItemsCompleted = orderItem.Item2.Where(oi => oi.StatusId == EnumOrderItemStatus.Completed);
                if (currentItemQuantity < orderedItemQuantity && orderItemsCompleted.Any())
                {
                    foreach (var item in orderItemsCompleted)
                    {
                        var preparedOrderItem = new GetProductCartItemResponse.PreparedOrderItemDto()
                        {
                            OrderItemId = item.Id,
                            ItemName = item.ItemName
                        };

                        preparedOrderItems.Add(preparedOrderItem);
                    }
                }
            }

            ThrowError.Against(preparedOrderItems.Any(), new JObject()
            {
                { MessageConstants.DIALOG_MESSAGE, string.Join(", ", preparedOrderItems.Select(i => i.ItemName).Distinct()) },
            });

        }
    }
}


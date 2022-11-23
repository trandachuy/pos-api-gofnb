using System;
using MediatR;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.POS.Models.Product;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Products.Queries
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

        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }
    }

    public class GetProductCartItemResponse
    {
        public List<ProductCartItemModel> CartItems { get; set; }

        public bool IsDiscountOnTotal { get; set; } = false;

        public decimal OriginalPrice { get; set; }

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

        public class PreparedOrderItemDto
        {
            public Guid OrderItemId { get; set; }

            public string ItemName { get; set; }
        }

        public Guid? PromotionId { get; set; }
    }

    public class GetProductCartItemRequestHandler : IRequestHandler<GetProductCartItemRequest, GetProductCartItemResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public GetProductCartItemRequestHandler(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            IOrderService orderService,
            ICustomerService customerService
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _orderService = orderService;
            _customerService = customerService;
        }

        public async Task<GetProductCartItemResponse> Handle(GetProductCartItemRequest request, CancellationToken cancellationToken)
        {
            var storeId = request.StoreId;
            var branchId = request.BranchId;

            ThrowError.Against(storeId == Guid.Empty);

            var maxPromotion = new Promotion();
            var response = new GetProductCartItemResponse()
            {
                CartItems = new List<ProductCartItemModel>()
            };

            if (!request.CartItems.Any())
            {
                await CalculateFeeAsync(request, response, storeId, maxPromotion);

                return response;
            };

            MergeProductCartItems(request);

            await CheckOrderItemStatusAsync(request, response);

            var productCartItems = new List<ProductCartItemModel>();
            var promotionsInStore = await _unitOfWork.Promotions
                .GetAllPromotionInStore(storeId)
                .Where(p => p.IsStopped != true)
                .AsNoTracking()
                .Include(p => p.PromotionProductCategories)
                .Include(p => p.PromotionProducts)
                .Include(item => item.PromotionBranches)
                .ToListAsync(cancellationToken: cancellationToken);

            /// Get all promotion activated today
            var today = _dateTimeService.NowUtc;
            var promotionsInStoreActivated = promotionsInStore.
                Where(p =>
                    today >= p.StartDate
                        && (p.EndDate == null || today.Date <= p.EndDate.Value.Date)
                );

            /// Find all DISCOUNT TOTAL BILL promotions for current branch
            var listPromotionTotalBill = promotionsInStoreActivated.
                Where(p =>
                    p.PromotionTypeId == (int)EnumPromotion.DiscountTotal &&
                    (p.IsSpecificBranch == false ||
                    (p.IsSpecificBranch == true &&
                        p.PromotionBranches.Any(pb => pb.BranchId == branchId)))
                );

            /// Find all DISCOUNT PRODUCT CATEGORY for current branch
            var listPromotionProductCategory = promotionsInStoreActivated.
                Where(p =>
                        p.PromotionTypeId == (int)EnumPromotion.DiscountProductCategory &&
                        (p.IsSpecificBranch == false ||
                        (p.IsSpecificBranch == true && p.PromotionBranches.Any(pb => pb.BranchId == branchId)))
                    );

            /// Find all DISCOUNT PRODUCT for current branch
            var listPromotionProduct = promotionsInStoreActivated.
                Where(p =>
                        p.PromotionTypeId == (int)EnumPromotion.DiscountProduct &&
                        (p.IsSpecificBranch == false ||
                            (p.IsSpecificBranch == true && p.PromotionBranches.Any(pb => pb.BranchId == branchId)))
                    );

            /// Get list product prices
            var productPriceIds = request.CartItems.Select(c => c.ProductPriceId);
            var productPrices = await _unitOfWork
                .ProductPrices
                .Find(p => productPriceIds.Any(id => id == p.Id))
                .Include(p => p.Product)
                    .ThenInclude(p => p.PromotionProducts)
                        .ThenInclude(p => p.Promotion)
                .Include(p => p.Product)
                    .ThenInclude(p => p.ProductProductCategories)
                        .ThenInclude(p => p.ProductCategory)
                            .ThenInclude(p => p.PromotionProductCategories)
                .Include(productPrice => productPrice.Product)
                .ThenInclude(item => item.Tax)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            /// Get list option levels by all option levels in cart items
            var optionLevelIds = request.CartItems.SelectMany(c => c.Options.Select(o => o.OptionLevelId));
            var optionLevels = await _unitOfWork.OptionLevels.Find(ol => optionLevelIds.Any(olid => olid == ol.Id))
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

            var promotionTypeSelected = EnumPromotion.DiscountTotal;
            decimal totalPriceInBill = 0;
            /// 1st. Discount on total bill
            if (listPromotionTotalBill.Any())
            {
                decimal totalOriginalPrice = 0;
                Parallel.ForEach(productPrices, p =>
                {
                    var aCartItem = request.CartItems.FirstOrDefault(x => x.ProductPriceId == p.Id);
                    if (aCartItem != null)
                    {
                        if (aCartItem.Toppings != null && aCartItem.Toppings.Count > 0)
                        {
                            var toppingList = aCartItem.Toppings.Where(topping => topping.Quantity > 0).ToList();
                            if (toppingList != null && toppingList.Count > 0)
                            {
                                toppingList.ForEach(topping =>
                                {
                                    totalOriginalPrice += topping.PriceValue * topping.Quantity;
                                });
                            }
                        }
                        totalOriginalPrice += p.PriceValue * aCartItem.Quantity;
                    }
                    else
                    {
                        totalOriginalPrice += p.PriceValue;
                    }
                });

                decimal discountValue = 0;
                maxPromotion = listPromotionTotalBill.FirstOrDefault();
                var combos = request.CartItems.Where(c => c.IsCombo);
                var priceCombos = request.CartItems.Where(c => c.IsCombo).Sum(item => item.Combo.SellingPrice * item.Quantity);
                var priceComboToppings = combos
                    .Sum(item => item.Quantity * item.Combo.ComboItems
                        .Sum(comboItem => comboItem.Toppings
                            .Sum(topping => topping.Quantity * topping.PriceAfterDiscount)));
                totalPriceInBill = totalOriginalPrice + priceCombos + priceComboToppings;
                foreach (var promotion in listPromotionTotalBill)
                {
                    var minimumRequired = promotion.IsMinimumPurchaseAmount == true ? promotion.MinimumPurchaseAmount : 0;
                    var maxDiscount = CalculatingDiscount(totalPriceInBill, promotion);
                    if (maxDiscount >= discountValue && totalPriceInBill >= minimumRequired)
                    {
                        maxPromotion = promotion;
                        discountValue = maxDiscount;
                        response.PromotionId = promotion.Id;
                        promotionTypeSelected = EnumPromotion.DiscountTotal;
                    }
                }

                if (response.PromotionId != null)
                {
                    var cartOrderItems = request.CartItems.Where(c => c.IsCombo == false);
                    foreach (var cartItem in cartOrderItems)
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        if (productPrice == null) continue;
                        var productCartItem = GetProductCartItemModel(productPrice, cartItem, maxPromotion, toppings, optionLevels, totalPriceInBill);
                        productCartItems.Add(productCartItem);
                    }

                    var totalBillAmount = CalculatingTotalOriginalPricelOnBill(productCartItems);
                    var totalPricelAfterDiscountOnBill = CalculatingTotalPricelAfterDiscountOnBill(productCartItems);
                    response.CartItems = productCartItems;
                    response.IsDiscountOnTotal = true;
                    response.OriginalPrice = totalBillAmount;

                    /// Find membership discount value
                    if (request.CustomerId.HasValue)
                    {
                        var membershipLevel = await _customerService.GetCustomerMembershipByCustomerIdAsync(request.CustomerId.Value, storeId);
                        var membershipDiscountValue = await _customerService.GetCustomerMembershipDiscountValueByCustomerIdAsync(response.OriginalPrice, request.CustomerId.Value, storeId);

                        totalPricelAfterDiscountOnBill -= membershipDiscountValue;
                        response.CustomerDiscountAmount = membershipDiscountValue;

                        if (membershipLevel != null)
                        {
                            response.CustomerMemberShipLevel = membershipLevel.Name;
                        }
                    }

                    response.TotalPriceAfterDiscount = totalPricelAfterDiscountOnBill;

                    if (maxPromotion.MaximumDiscountAmount > 0 && (totalBillAmount - totalPricelAfterDiscountOnBill) > maxPromotion.MaximumDiscountAmount)
                    {
                        response.TotalPriceAfterDiscount = totalBillAmount - maxPromotion.MaximumDiscountAmount;
                    }

                    response.DiscountTotalPromotion = new GetProductCartItemResponse.PromotionDto()
                    {
                        Id = maxPromotion.Id,
                        Name = maxPromotion.Name,
                        DiscountValue = (totalBillAmount - totalPricelAfterDiscountOnBill) > maxPromotion.MaximumDiscountAmount && maxPromotion.MaximumDiscountAmount > 0
                            ? maxPromotion.MaximumDiscountAmount
                            : totalBillAmount - totalPricelAfterDiscountOnBill,
                        MaximumDiscountAmount = maxPromotion.MaximumDiscountAmount
                    };
                }
            }

            if (!response.PromotionId.HasValue)
            {
                /// 2nd. Discount on specific category
                var discountOnProductCategoryPromotions = listPromotionProductCategory.SelectMany(p => p.PromotionProductCategories);
                var listProductCategoryIdsByPromotion = discountOnProductCategoryPromotions?.Select(p => p.ProductCategoryId).Distinct();
                if (discountOnProductCategoryPromotions.Any() && listProductCategoryIdsByPromotion.Any())
                {
                    var listProductCategoryPromotionAppied = new List<Tuple<Guid, Promotion, decimal>>();
                    foreach (var productCategoryId in listProductCategoryIdsByPromotion)
                    {
                        var listProductPricesByCategory = productPrices.Where(pprice => pprice.Product.ProductProductCategories.Any(ppc => ppc.ProductCategoryId == productCategoryId));
                        var promotions = discountOnProductCategoryPromotions.Where(promotion => promotion.ProductCategoryId == productCategoryId).Select(p => p.Promotion);
                        var maxPromotionApplied = promotions.FirstOrDefault();
                        decimal maxPriceDiscountApplied = 0;
                        if (promotions.Any())
                        {
                            foreach (var promotion in promotions)
                            {
                                var minimumRequired = promotion.IsMinimumPurchaseAmount == true ? promotion.MinimumPurchaseAmount : 0;
                                foreach (var proPrice in listProductPricesByCategory)
                                {
                                    var maxPriceDiscount = CalculatingDiscount(proPrice.PriceValue, promotion);
                                    if (maxPriceDiscount >= maxPriceDiscountApplied
                                        && proPrice.PriceValue >= minimumRequired
                                        && response.DiscountTotalPromotion == null || maxPriceDiscount > response.DiscountTotalPromotion.DiscountValue)
                                    {
                                        maxPriceDiscountApplied = maxPriceDiscount;
                                        maxPromotionApplied = promotion;
                                        response.PromotionId = promotion.Id;
                                        promotionTypeSelected = EnumPromotion.DiscountProductCategory;
                                    }
                                }
                            }
                        }

                        var productCategoryAndPromotion = new Tuple<Guid, Promotion, decimal>(productCategoryId, maxPromotionApplied, maxPriceDiscountApplied);
                        listProductCategoryPromotionAppied.Add(productCategoryAndPromotion);
                    }

                    if (promotionTypeSelected == EnumPromotion.DiscountProductCategory)
                    {
                        var cartOrderItems = request.CartItems.Where(c => c.IsCombo == false);
                        foreach (var cartItem in cartOrderItems)
                        {
                            var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                            if (productPrice == null) continue;

                            var productProductCategories = productPrice.Product.ProductProductCategories;
                            var listProductCategoryPromotionAppy = listProductCategoryPromotionAppied.Where(p => productProductCategories.Any(pcid => pcid.ProductCategoryId == p.Item1));
                            if (listProductCategoryPromotionAppy.Any())
                            {
                                var promotion = listProductCategoryPromotionAppy.Aggregate((currentMax, next) => currentMax == null || next.Item3 > currentMax.Item3 ? next : currentMax);
                                var productCartItem = GetProductCartItemModel(productPrice, cartItem, promotion.Item2, toppings, optionLevels);

                                productCartItems.Add(productCartItem);
                            }
                            else
                            {
                                var productPromotionAppied = new List<Tuple<Guid, Promotion, decimal>>();
                                var discountOnSpecificProductPromotions = listPromotionProduct.SelectMany(p => p.PromotionProducts);
                                var productId = productPrice.ProductId;
                                var listProductPricesByProductId = productPrices.Where(pprice => pprice.ProductId == productId);
                                var promotions = discountOnSpecificProductPromotions.Where(promotion => promotion.ProductId == productId).Select(p => p.Promotion);
                                var maxPromotionApplied = promotions.FirstOrDefault();
                                decimal maxPriceDiscountApplied = 0;
                                if (promotions.Any())
                                {
                                    foreach (var promotion in promotions)
                                    {
                                        var minimumRequired = promotion.IsMinimumPurchaseAmount == true ? promotion.MinimumPurchaseAmount : 0;
                                        foreach (var proPrice in listProductPricesByProductId)
                                        {
                                            var maxPriceDiscount = CalculatingDiscount(proPrice.PriceValue, promotion);
                                            if (maxPriceDiscount >= maxPriceDiscountApplied && proPrice.PriceValue >= minimumRequired)
                                            {
                                                maxPriceDiscountApplied = maxPriceDiscount;
                                                maxPromotionApplied = promotion;
                                                response.PromotionId = promotion.Id;
                                            }
                                        }
                                    }
                                }

                                var productCategoryAndPromotion = new Tuple<Guid, Promotion, decimal>(productId, maxPromotionApplied, maxPriceDiscountApplied);
                                productPromotionAppied.Add(productCategoryAndPromotion);
                                var listProductPromotionAppy = productPromotionAppied.Where(p => p.Item1 == productId);
                                GetProductCartItems(productCartItems, cartItem, listProductPromotionAppy, productPrice, toppings, optionLevels);
                            }
                        }
                    }
                }
            }

            if (!response.PromotionId.HasValue)
            {
                /// 3rd. Discount on specific product
                var listProductPromotionAppied = new List<Tuple<Guid, Promotion, decimal>>();
                var listDiscountOnSpecificProductPromotions = listPromotionProduct.SelectMany(p => p.PromotionProducts);
                var listProductIdsByPromotion = listDiscountOnSpecificProductPromotions.Select(p => p.ProductId).Distinct();
                foreach (var productId in listProductIdsByPromotion)
                {
                    var listProductPricesByProductId = productPrices.Where(pprice => pprice.ProductId == productId);
                    var promotions = listDiscountOnSpecificProductPromotions.Where(promotion => promotion.ProductId == productId).Select(p => p.Promotion);
                    var maxPromotionApplied = promotions.FirstOrDefault();
                    decimal maxPriceDiscountApplied = 0;

                    foreach (var promotion in promotions)
                    {
                        var minimumRequired = promotion.IsMinimumPurchaseAmount == true ? promotion.MinimumPurchaseAmount : 0;
                        foreach (var proPrice in listProductPricesByProductId)
                        {
                            var maxPriceDiscount = CalculatingDiscount(proPrice.PriceValue, promotion);
                            if (maxPriceDiscount >= maxPriceDiscountApplied
                                && proPrice.PriceValue >= minimumRequired
                                && response.DiscountTotalPromotion == null || maxPriceDiscount > response.DiscountTotalPromotion.DiscountValue)
                            {
                                maxPriceDiscountApplied = maxPriceDiscount;
                                maxPromotionApplied = promotion;
                                response.PromotionId = promotion.Id;
                                promotionTypeSelected = EnumPromotion.DiscountProduct;
                            }
                        }
                    }

                    var productCategoryAndPromotion = new Tuple<Guid, Promotion, decimal>(productId, maxPromotionApplied, maxPriceDiscountApplied);
                    listProductPromotionAppied.Add(productCategoryAndPromotion);
                }

                if (promotionTypeSelected == EnumPromotion.DiscountProduct || response.PromotionId == null)
                {
                    foreach (var cartItem in request.CartItems)
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        if (productPrice == null) continue;

                        var productId = productPrice.ProductId;
                        var listProductPromotionAppy = listProductPromotionAppied.Where(p => p.Item1 == productId);
                        GetProductCartItems(productCartItems, cartItem, listProductPromotionAppy, productPrice, toppings, optionLevels);
                    }
                }
            }

            response.CartItems = productCartItems;
            response.IsDiscountOnTotal = false;
            response.OriginalPrice = CalculatingTotalOriginalPricelOnBill(productCartItems);
            var totalPriceAfterDiscount = CalculatingTotalPricelAfterDiscount(productCartItems, maxPromotion);

            /// Find membership discount value
            if (request.CustomerId.HasValue)
            {
                var membershipDiscountValue = await _customerService.GetCustomerMembershipDiscountValueByCustomerIdAsync(response.OriginalPrice, request.CustomerId.Value, storeId);
                totalPriceAfterDiscount -= membershipDiscountValue;
                response.CustomerDiscountAmount = membershipDiscountValue;

                response.CustomerId = request.CustomerId;
            }

            response.TotalPriceAfterDiscount = totalPriceAfterDiscount;

            await ComboHandlerAsync(request, response, storeId, totalPriceInBill, maxPromotion);

            await CalculateFeeAsync(request, response, storeId, maxPromotion);

            await CalculateTaxAsync(request, response);

            return response;
        }

        /// <summary>
        /// Mapping product price, promotion, topping, option to ProductCartItemModel
        /// </summary>
        /// <param name="result"></param>
        /// <param name="cartItem"></param>
        /// <param name="listPromotions"></param>
        /// <param name="productPrice"></param>
        /// <param name="toppings"></param>
        /// <param name="optionLevels"></param>
        private static void GetProductCartItems(
            List<ProductCartItemModel> result,
            OrderCartItemRequestModel cartItem,
            IEnumerable<Tuple<Guid, Promotion, decimal>> listPromotions,
            ProductPrice productPrice,
            List<Product> toppings,
            List<OptionLevel> optionLevels
            )
        {
            if (listPromotions.Any())
            {
                var maxPromotion = listPromotions.Aggregate((currentMax, next) => currentMax == null || next.Item3 > currentMax.Item3 ? next : currentMax);
                var productCartItem = GetProductCartItemModel(productPrice, cartItem, maxPromotion.Item2, toppings, optionLevels);
                result.Add(productCartItem);
            }
            else
            {
                var productCartItem = GetProductCartItemModel(productPrice, cartItem, null, toppings, optionLevels);
                result.Add(productCartItem);
            }
        }

        /// <summary>
        /// Mapping topping with promotion to ToppingDto
        /// </summary>
        /// <param name="promotion"></param>
        /// <param name="allToppings"></param>
        /// <param name="requestToppings"></param>
        /// <returns></returns>
        private static List<ProductCartItemModel.ToppingDto> GetToppingsDataFromRequest(
            Promotion promotion,
            List<Product> allToppings,
            IEnumerable<ProductToppingModel> requestToppings,
            int itemQuanity)
        {
            var result = new List<ProductCartItemModel.ToppingDto>();
            foreach (var requestTopping in requestToppings)
            {
                var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);
                if (topping == null) continue;

                var toppingPrice = topping.ProductPrices.FirstOrDefault();
                var originalPrice = ((toppingPrice?.PriceValue ?? 0) * requestTopping.Quantity) * itemQuanity;
                var priceAfterDiscount = promotion != null && promotion.IsIncludedTopping ? CalculatingPriceAfterDiscount(originalPrice, CalculatingDiscount(originalPrice, promotion)) : originalPrice;
                var toppingDto = new ProductCartItemModel.ToppingDto()
                {
                    ToppingId = topping.Id,
                    PromotionId = promotion?.Id,
                    Name = topping.Name,
                    Price = topping?.ProductPrices.FirstOrDefault()?.PriceValue ?? 0,
                    Quantity = requestTopping.Quantity,
                    OriginalPrice = originalPrice,
                    PriceAfterDiscount = priceAfterDiscount
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
                if (optionLevel == null) continue;

                var optionLevelDto = new ProductOptionDto()
                {
                    OptionId = optionLevel.OptionId,
                    OptionLevelId = optionLevel.Id,
                    OptionName = optionLevel.Option.Name,
                    OptionLevelName = optionLevel.Name
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
        private static decimal CalculatingDiscount(decimal price, Promotion promotion)
        {
            if (promotion.IsPercentDiscount)
            {
                decimal discountValue = 0;
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (price >= promotion.MinimumPurchaseAmount.Value)
                    {
                        discountValue = price * (decimal)promotion.PercentNumber / 100;
                    }
                }
                else
                {
                    discountValue = price * (decimal)promotion.PercentNumber / 100;
                }

                if (promotion.MaximumDiscountAmount == 0)
                {
                    return discountValue;
                }

                return discountValue >= promotion.MaximumDiscountAmount ? promotion.MaximumDiscountAmount : discountValue;
            }
            else
            {
                return promotion.MaximumDiscountAmount;
            }
        }

        /// <summary>
        /// Calculating Discount For Total Bill
        /// </summary>
        /// <param name="price"></param>
        /// <param name="totalOriginalPrice"></param>
        /// <param name="promotion"></param>
        /// <param name="toppingsOfItem"></param>
        /// <returns></returns>
        private static decimal CalculatingDiscountForTotalBill(decimal price, decimal totalOriginalPrice, Promotion promotion, List<ProductCartItemModel.ToppingDto> toppingsOfItem)
        {
            if (promotion.IsPercentDiscount)
            {
                decimal discountValue = 0;
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (totalOriginalPrice >= promotion.MinimumPurchaseAmount.Value)
                    {
                        discountValue = price * (decimal)promotion.PercentNumber / 100;
                        if (promotion.IsIncludedTopping)
                        {
                            var priceOfToppings = toppingsOfItem.Sum(t => t.PriceAfterDiscount);
                            var priceOfToppingsDiscount = priceOfToppings * (decimal)promotion.PercentNumber / 100;
                            discountValue += priceOfToppingsDiscount;
                        }
                    }
                }
                else
                {
                    discountValue = price * (decimal)promotion.PercentNumber / 100;
                }

                if (promotion.MaximumDiscountAmount == 0)
                {
                    return discountValue;
                }

                return discountValue >= promotion.MaximumDiscountAmount ? promotion.MaximumDiscountAmount : discountValue;
            }
            else
            {
                return promotion.MaximumDiscountAmount;
            }
        }

        /// <summary>
        /// Calculating price after discount with promotion
        /// </summary>
        /// <param name="price"></param>
        /// <param name="promotion"></param>
        /// <param name="toppingsOfItem"></param>
        /// <returns></returns>
        private static decimal CalculatingAllPriceAfterDiscount(decimal price, Promotion promotion, List<ProductCartItemModel.ToppingDto> toppingsOfItem)
        {
            var priceOfToppings = toppingsOfItem.Sum(t => t.PriceAfterDiscount);
            if (promotion.IsPercentDiscount)
            {
                decimal discountValue = 0;
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (price >= promotion.MinimumPurchaseAmount.Value)
                    {
                        discountValue = price * (decimal)promotion.PercentNumber / 100;
                    }
                }
                else
                {
                    discountValue = price * (decimal)promotion.PercentNumber / 100;
                }

                if (promotion.IsIncludedTopping)
                {
                    discountValue = CalculatingPriceAfterDiscount(discountValue, priceOfToppings);
                }

                var maxDiscountValue = ((promotion.MaximumDiscountAmount != 0) && discountValue >= promotion.MaximumDiscountAmount) ? promotion.MaximumDiscountAmount : discountValue;
                var result = CalculatingPriceAfterDiscount(price, maxDiscountValue);

                return result;
            }
            else
            {
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (price < promotion.MinimumPurchaseAmount.Value)
                    {
                        return price;
                    }
                }

                var discountValue = promotion.MaximumDiscountAmount;
                if (promotion.IsIncludedTopping)
                {
                    discountValue = CalculatingPriceAfterDiscount(discountValue, priceOfToppings);
                }

                var result = CalculatingPriceAfterDiscount(price, discountValue);

                return result;
            }
        }

        /// <summary>
        /// Calculating All Price After Discount Of Total Bill
        /// </summary>
        /// <param name="price"></param>
        /// <param name="totalPrice"></param>
        /// <param name="promotion"></param>
        /// <param name="toppingsOfItem"></param>
        /// <returns></returns>
        private static decimal CalculatingAllPriceAfterDiscountOfTotalBill(
            decimal price,
            decimal totalPrice,
            Promotion promotion,
            List<ProductCartItemModel.ToppingDto> toppingsOfItem)
        {
            var priceOfToppings = toppingsOfItem.Sum(t => t.PriceAfterDiscount);
            if (promotion.IsPercentDiscount)
            {
                decimal discountValue = 0;
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (totalPrice >= promotion.MinimumPurchaseAmount.Value)
                    {
                        discountValue = price * (decimal)promotion.PercentNumber / 100;

                        if (promotion.IsIncludedTopping && priceOfToppings > 0)
                        {
                            var afterPriceTopping = priceOfToppings * (decimal)promotion.PercentNumber / 100;
                            discountValue += afterPriceTopping;
                        }
                    }
                }
                else
                {
                    discountValue = price * (decimal)promotion.PercentNumber / 100;
                }

                var maxDiscountValue = (promotion.MaximumDiscountAmount > 0 && discountValue >= promotion.MaximumDiscountAmount)
                    ? promotion.MaximumDiscountAmount
                    : discountValue;
                var result = CalculatingPriceAfterDiscount(price, maxDiscountValue);

                return result;
            }
            else
            {
                if (promotion.IsMinimumPurchaseAmount.HasValue &&
                    promotion.IsMinimumPurchaseAmount.Value)
                {
                    if (totalPrice < promotion.MinimumPurchaseAmount.Value)
                    {
                        return price;
                    }
                }

                var discountValue = promotion.MaximumDiscountAmount;
                if (promotion.IsIncludedTopping)
                {
                    var afterPriceTopping = priceOfToppings * (decimal)promotion.PercentNumber / 100;
                    discountValue += afterPriceTopping;
                }

                var result = CalculatingPriceAfterDiscount(price, discountValue);
                return result;
            }
        }

        private static decimal CalculatingPriceAfterDiscount(decimal price, decimal discountValue)
        {
            var priceAfterDiscount = price - discountValue;

            return priceAfterDiscount <= 0 ? 0 : priceAfterDiscount;
        }

        private static decimal CalculatingTotalOriginalPricelOnBill(List<ProductCartItemModel> cartItems)
        {
            decimal totalBillAmount = cartItems.Sum(i => i.OriginalPrice);
            decimal totalPriceTopping = 0;

            foreach (var item in cartItems)
            {
                if (item.Toppings == null || !item.Toppings.Any()) continue;

                var priceOfToppings = item.Toppings.Sum(t => t.OriginalPrice);
                totalPriceTopping += priceOfToppings;
            }
            decimal result = totalBillAmount + totalPriceTopping;

            return result;
        }

        private static decimal CalculatingTotalPricelAfterDiscountOnBill(List<ProductCartItemModel> cartItems)
        {
            decimal totalBillAmount = cartItems.Sum(i => i.PriceAfterDiscount);
            decimal totalPriceTopping = 0;

            foreach (var item in cartItems)
            {
                if (item.Toppings == null || !item.Toppings.Any()) continue;

                var priceOfToppings = item.Toppings.Sum(t => t.PriceAfterDiscount);
                totalPriceTopping += priceOfToppings;
            }
            decimal result = totalBillAmount + totalPriceTopping;

            return result;
        }

        private static decimal CalculatingTotalPricelAfterDiscount(List<ProductCartItemModel> cartItems, Promotion promotion)
        {
            decimal totalBillAmountAfterDisctount = cartItems.Sum(i => i.PriceAfterDiscount);
            decimal totalPriceTopping = 0;
            foreach (var item in cartItems)
            {
                if (item.Toppings == null || !item.Toppings.Any()) continue;

                var priceOfToppings = item.Toppings.Sum(t => t.PriceAfterDiscount);
                totalPriceTopping += priceOfToppings;
            }

            decimal result = totalBillAmountAfterDisctount + totalPriceTopping;

            return result;
        }

        /// <summary>
        /// Mapping to product cart item model
        /// </summary>
        /// <param name="productPrice"></param>
        /// <param name="cartItem"></param>
        /// <param name="promotion"></param>
        /// <param name="toppings"></param>
        /// <param name="optionLevels"></param>
        /// <param name="totalOriginalPrice"></param>
        /// <returns></returns>
        private static ProductCartItemModel GetProductCartItemModel(
            ProductPrice productPrice,
            OrderCartItemRequestModel cartItem,
            Promotion promotion,
            List<Product> toppings,
            List<OptionLevel> optionLevels,
            decimal totalOriginalPrice = 0m)
        {
            var originalPrice = productPrice.PriceValue * cartItem.Quantity;
            var productCartItem = new ProductCartItemModel()
            {
                OrderItemId = cartItem.OrderItemId,
                ProductPriceId = productPrice.Id,
                ItemName = $"{productPrice.Product.Name} {productPrice.PriceName}",
                OriginalPrice = originalPrice,
                PriceAfterDiscount = originalPrice,
                Quantity = cartItem.Quantity,
                ProductPriceName = productPrice.PriceName,
                ProductId = cartItem.ProductId,
                OrderId = cartItem.OrderId ?? Guid.Empty,
                Notes = cartItem.Notes,
            };

            var itemToppings = GetToppingsDataFromRequest(promotion, toppings, cartItem.Toppings, cartItem.Quantity);
            var itemOptions = GetOptionsDataFromRequest(optionLevels, cartItem.Options);
            if (promotion != null)
            {
                var priceAfterDiscountWithPromotion = 0m;
                var isPromotionTotalBill = (promotion.PromotionTypeId == (int)EnumPromotion.DiscountTotal);
                if (isPromotionTotalBill)
                {
                    priceAfterDiscountWithPromotion = CalculatingAllPriceAfterDiscountOfTotalBill(originalPrice, totalOriginalPrice, promotion, itemToppings);
                }
                else
                {
                    priceAfterDiscountWithPromotion = CalculatingAllPriceAfterDiscount(originalPrice, promotion, itemToppings);
                }

                productCartItem.Promotion = new ProductCartItemModel.PromotionDto()
                {
                    Id = promotion.Id,
                    PromotionTypeId = promotion.PromotionTypeId,
                    Name = promotion.Name,
                    IsPercentDiscount = promotion.IsPercentDiscount,
                    PercentNumber = (decimal)promotion.PercentNumber,
                    DiscountValue = originalPrice - priceAfterDiscountWithPromotion
                };

                var discountValue = 0m;

                if (isPromotionTotalBill)
                {
                    discountValue = CalculatingDiscountForTotalBill(originalPrice, totalOriginalPrice, promotion, itemToppings);
                }
                else
                {
                    discountValue = CalculatingDiscount(originalPrice, promotion);
                }

                var productPriceAfterDiscount = CalculatingPriceAfterDiscount(originalPrice, discountValue);
                productCartItem.PriceAfterDiscount = productPriceAfterDiscount;
            }

            productCartItem.Toppings = itemToppings;
            productCartItem.Options = itemOptions;

            return productCartItem;
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
                  .ToListAsync();

                if (fees.Count > 0)
                {
                    totalFeeValue = _orderService.CalculateTotalFeeValue(originalPrice, fees);
                }
            }

            return totalFeeValue;
        }

        private async Task CalculateFeeAsync(
            GetProductCartItemRequest request,
            GetProductCartItemResponse response,
            Guid storeId,
            Promotion maxPromotion)
        {
            #region Handle calculate OriginalPrice and TotalPriceAfterDiscount when combo included
            var combos = response.CartItems.Where(c => c.IsCombo == true).Select(c => c.Combo);
            if (response.CartItems.Any() && combos.Any())
            {
                decimal totalComboSellingPriceValue = 0;
                decimal totalComboOriginalPriceValue = 0;
                decimal totalComboSellingPriceAfterDiscountValue = 0;

                if (!maxPromotion.IsPercentDiscount)
                {
                    totalComboSellingPriceAfterDiscountValue = response.OriginalPrice - response.TotalPriceAfterDiscount;
                }

                foreach (var combo in combos)
                {
                    var toppings = combo.ComboItems.SelectMany(i => i.Toppings);
                    var toppingPriceValue = toppings.Sum(t => t.PriceAfterDiscount) * combo.Quantity;

                    var comboSellingPriceAfterDiscountValue = combo.SellingPriceAfterDiscount * combo.Quantity;
                    var comboSellingPriceValue = combo.SellingPrice * combo.Quantity;
                    var comboOriginalPriceValue = combo.OriginalPrice * combo.Quantity;
                    totalComboSellingPriceValue += comboSellingPriceValue + toppingPriceValue;
                    totalComboOriginalPriceValue += comboOriginalPriceValue + toppingPriceValue;
                    totalComboSellingPriceAfterDiscountValue += comboSellingPriceAfterDiscountValue + toppingPriceValue;
                }

                response.OriginalPrice += totalComboOriginalPriceValue;

                if (maxPromotion != null
                    && maxPromotion.IsMinimumPurchaseAmount.HasValue
                    && (response.OriginalPrice > maxPromotion.MinimumPurchaseAmount)
                    && (maxPromotion.MaximumDiscountAmount > 0)
                    && ((response.OriginalPrice - totalComboSellingPriceAfterDiscountValue) > maxPromotion.MaximumDiscountAmount))
                {
                    response.TotalPriceAfterDiscount = response.OriginalPrice - maxPromotion.MaximumDiscountAmount - (totalComboOriginalPriceValue - totalComboSellingPriceValue);
                }
                else
                {
                    response.TotalPriceAfterDiscount += totalComboSellingPriceAfterDiscountValue;
                }
            }
            #endregion

            var listFeeIdByServingTypes = _unitOfWork
                .FeeServingTypes
                .Find(item => item.StoreId == storeId && (int)item.OrderServingType == 3)
                .Select(item => item.FeeId);

            var today = _dateTimeService.NowUtc;
            var listFeeIds = _unitOfWork.FeeBranches
                .Find(item => item.StoreId == storeId
                    && item.BranchId == request.BranchId
                    && listFeeIdByServingTypes.Contains(item.FeeId)
                    && (item.Fee.IsStopped == null || !item.Fee.IsStopped.Value)
                    && item.Fee.StartDate.Value.Date <= today.Date
                    && item.Fee.EndDate == null || item.Fee.EndDate.Value.Date >= today.Date)
                .Select(item => item.FeeId);

            var totalFee = await CalculateTotalFeeAsync(response.OriginalPrice, listFeeIds, storeId);
            if (totalFee > 0)
            {
                response.TotalFee = totalFee;
                response.TotalPriceAfterDiscount += totalFee;
            }
        }

        private async Task CalculateTaxAsync(GetProductCartItemRequest request, GetProductCartItemResponse response)
        {
            var productPrices = await _unitOfWork.ProductPrices
                .GetAllProductPriceInStore(request.StoreId)
                .AsNoTracking()
                .ToListAsync();

            if (productPrices == null)
            {
                return;
            }

            decimal totalTax = 0m;
            foreach (var cartItem in request.CartItems)
            {
                if (cartItem.IsCombo)
                {
                    foreach (var combo in cartItem.Combo.ComboItems)
                    {
                        var productPrice = productPrices.FirstOrDefault(productPrice => productPrice.Id == combo.ProductPriceId);
                        if (productPrice == null) continue;
                        var comboTax = productPrice?.Product?.Tax?.Percentage != null
                            ? (productPrice?.Product?.Tax?.Percentage * combo.Quantity * productPrice.PriceValue) / 100
                            : 0;
                        totalTax += comboTax.Value;
                    }
                }
                else
                {
                    var productPrice = productPrices.FirstOrDefault(productPrice => productPrice.Id == cartItem.ProductPriceId);
                    if (productPrice == null) continue;
                    var taxValue = (productPrice?.Product?.Tax?.Percentage != null)
                        ? (productPrice?.Product?.Tax?.Percentage * productPrice.PriceValue * cartItem.Quantity) / 100
                        : 0;
                    totalTax += taxValue.Value;
                }
            }

            if (totalTax > 0)
            {
                response.TotalTax = totalTax;
                response.TotalPriceAfterDiscount += totalTax;
            }
        }

        private async Task ComboHandlerAsync(
            GetProductCartItemRequest request,
            GetProductCartItemResponse response,
            Guid storeId,
            decimal totalPrice,
            Promotion promotion)
        {
            var combos = request.CartItems.Where(c => c.IsCombo == true).Select(c => c.Combo).Where(c => c.ComboItems != null);
            var productPrices = await _unitOfWork.ProductPrices
                .GetAllProductPriceInStore(storeId)
                .AsNoTracking()
                .ToListAsync();

            var allOptionLevelIds = combos.SelectMany(c => c.ComboItems.SelectMany(i => i.Options.Select(o => o.OptionLevelId)));

            var allOptionLevels = await _unitOfWork.OptionLevels
                .Find(o => allOptionLevelIds.Any(oid => oid == o.Id))
                .Include(o => o.Option)
                .AsNoTracking()
                .ToListAsync();

            var allToppingIds = combos.SelectMany(c => c.ComboItems.SelectMany(i => i.Toppings.Select(o => o.ToppingId)));
            var allToppings = await _unitOfWork.Products
                .Find(o => allToppingIds.Any(oid => oid == o.Id))
                .Include(t => t.ProductPrices)
                .AsNoTracking()
                .ToListAsync();

            foreach (var combo in combos)
            {
                if (combo.ComboItems == null) continue;

                var comboDto = new ComboOrderItemDto()
                {
                    ComboId = combo.ComboId,
                    ComboPricingId = combo.ComboPricingId,
                    ComboName = combo.ComboName,
                    ItemName = combo.ItemName,
                    OriginalPrice = combo.OriginalPrice,
                    SellingPrice = combo.SellingPrice,
                    Quantity = combo.Quantity,
                    Notes = combo.Notes,
                    ComboItems = new List<ComboOrderItemDto.ComboItemDto>()
                };

                foreach (var item in combo.ComboItems)
                {
                    var productPrice = productPrices.FirstOrDefault(p => p.Id == item.ProductPriceId);
                    if (productPrice == null) continue;

                    var newComboItem = new ComboOrderItemDto.ComboItemDto()
                    {
                        ProductPriceId = item.ProductPriceId,
                        ItemName = $"{productPrice.Product.Name} {productPrice.PriceName}",
                        Quantity = item.Quantity > 0 ? item.Quantity : 1,
                        Toppings = new List<ComboOrderItemDto.ProductToppingDto>(),
                        Options = new List<ProductOptionDto>(),
                        Note = item.Note,
                    };

                    item.Toppings.ForEach(requestTopping =>
                    {
                        var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);

                        /// If the topping price is not found, continues
                        if (topping == null) return;

                        /// The topping as a product and the product has multiple prices but the topping ONLY one price.
                        var toppingPrice = topping.ProductPrices.FirstOrDefault();

                        /// The topping price in combo will be not apply promotion.
                        var originalPrice = (toppingPrice?.PriceValue ?? 0) * requestTopping.Quantity;
                        var toppingDto = new ComboOrderItemDto.ProductToppingDto()
                        {
                            ToppingId = topping.Id,
                            Name = topping.Name,
                            Quantity = requestTopping.Quantity,
                            OriginalPrice = originalPrice,
                            PriceAfterDiscount = originalPrice
                        };

                        newComboItem.Toppings.Add(toppingDto);
                    });

                    item.Options.ForEach(requestOption =>
                    {
                        var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == requestOption.OptionLevelId);

                        /// If the option level is not found, continues
                        if (optionLevel == null) return;

                        var optionLevelDto = new ProductOptionDto()
                        {
                            OptionId = optionLevel.OptionId,
                            OptionLevelId = optionLevel.Id,
                            OptionName = optionLevel.Option.Name,
                            OptionLevelName = optionLevel.Name
                        };

                        newComboItem.Options.Add(optionLevelDto);
                    });

                    comboDto.ComboItems.Add(newComboItem);
                }

                var toppings = comboDto.ComboItems
                    .SelectMany(item => item.Toppings
                        .Select(topping => new ProductCartItemModel.ToppingDto
                        {
                            PriceAfterDiscount = topping.PriceAfterDiscount,
                            Quantity = combo.Quantity,
                        })).ToList();

                comboDto.SellingPriceAfterDiscount = CalculatingAllPriceAfterDiscountOfTotalBill(
                    combo.SellingPrice,
                    totalPrice,
                    promotion,
                    toppings);

                var newProductCartItem = new ProductCartItemModel()
                {
                    IsCombo = true,
                    Combo = comboDto,
                    Quantity = combo.Quantity
                };

                response.CartItems.Add(newProductCartItem);
            }
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

            request.CartItems = result;
        }

        private static OrderCartItemRequestModel GetProductItemDuplicated(OrderCartItemRequestModel item, List<OrderCartItemRequestModel> result)
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = (item.ProductPriceId == existed.ProductPriceId) && item.ProductPriceId != Guid.Empty;
                var isOptionDuplicated = item.Options.All(o => existed.Options.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.Toppings.All(o => existed.Toppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity));
                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
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
        private async Task CheckOrderItemStatusAsync(GetProductCartItemRequest request, GetProductCartItemResponse response)
        {
            var orderItemId = request.CartItems.Where(c => c.OrderItemId != null).Select(c => c.OrderItemId.Value).FirstOrDefault();
            if (orderItemId == Guid.Empty) return;

            var firstOrderItem = await _unitOfWork.OrderItems.Find(oi => oi.Id == orderItemId).FirstOrDefaultAsync();
            var orderItems = await _unitOfWork.OrderItems
                .Find(oi => oi.OrderId == firstOrderItem.OrderId)
                .Include(oi => oi.OrderItemOptions)
                .Include(oi => oi.OrderItemToppings)
                .AsNoTracking()
                .ToListAsync();

            var mergedSameOrderItems = _orderService.MergeSameOrderItems(orderItems);

            var preparedOrderItems = new List<GetProductCartItemResponse.PreparedOrderItemDto>();
            foreach (var orderItem in mergedSameOrderItems)
            {
                var requestItem = request.CartItems.FirstOrDefault(c => c.OrderItemId == orderItem.Item1.Id);
                if (requestItem == null) continue;

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

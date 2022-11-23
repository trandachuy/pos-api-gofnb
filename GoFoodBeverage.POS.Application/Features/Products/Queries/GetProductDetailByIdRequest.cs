using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Product;
using GoFoodBeverage.POS.Models.Promotion;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetProductDetailByIdRequest : IRequest<GetProductDetailByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetProductDetailByIdResponse
    {
        public ProductDetailByIdModel ProductDetail { get; set; }

        public IEnumerable<ProductToppingModel> ProductToppings { get; set; }
    }

    public class GetProductDetailByIdRequestHandler : IRequestHandler<GetProductDetailByIdRequest, GetProductDetailByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUserProvider _userProvider;

        public GetProductDetailByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _userProvider = userProvider;
        }

        public async Task<GetProductDetailByIdResponse> Handle(GetProductDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productDetailData = await _unitOfWork.Products
                .Find(p => p.StoreId == loggedUser.StoreId && p.Id == request.Id)
                .AsNoTracking()
                .Include(x => x.ProductOptions).ThenInclude(x => x.Option).ThenInclude(x => x.OptionLevel)
                .Include(x => x.ProductPrices)
                .Include(p => p.ProductProductCategories)
                .Select(product => new ProductDetailByIdModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Thumbnail = product.Thumbnail,
                    ProductPrices = product.ProductPrices.Select(p => new ProductDetailByIdModel.ProductPriceDto
                    {
                        Id = p.Id,
                        PriceName = p.PriceName,
                        PriceValue = p.PriceValue,
                        CreatedTime = p.CreatedTime
                    })
                    .OrderBy(p => p.CreatedTime).ToList(),
                    ProductOptions = product.ProductOptions.Select(p => new ProductDetailByIdModel.ProductOptionDto
                    {
                        Id = p.OptionId,
                        Name = p.Option.Name,
                        OptionLevels = p.Option.OptionLevel.Select(o => new ProductDetailByIdModel.ProductOptionDto.OptionLevelDto
                        {
                            Id = o.Id,
                            Name = o.Name,
                            IsSetDefault = o.IsSetDefault,
                            OptionId = o.OptionId,
                        })
                        .OrderByDescending(o => o.IsSetDefault)
                    }),
                    ProductCategoryId = product.ProductProductCategories.FirstOrDefault().ProductCategoryId
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.Against(productDetailData == null, "Cannot find product detail information");

            DateTime today = _dateTimeService.NowUtc;
            var productPromotions = await _unitOfWork.PromotionProducts
               .GetAll()
               .AsNoTracking()
               .Include(p => p.Promotion)
               .Where(
               p => p.StoreId == loggedUser.StoreId 
               && p.ProductId == request.Id
               && p.Promotion.StartDate.CompareTo(today) <= 0
               && p.Promotion.EndDate.Value.CompareTo(today) >= 0
               && p.Promotion.IsStopped == false
               )
               .Select(p => new PromotionModel()
                {
                    Name = p.Promotion.Name,
                    IsPercentDiscount = p.Promotion.IsPercentDiscount,
                    PercentNumber = p.Promotion.PercentNumber,
                    MaximumDiscountAmount = p.Promotion.MaximumDiscountAmount,
               })
               .ToListAsync(cancellationToken: cancellationToken);

            var productCategoryPromotions = await _unitOfWork.PromotionProductCategories
               .GetAll()
               .AsNoTracking()
               .Include(p => p.Promotion)
               .Where(p =>
               p.StoreId == loggedUser.StoreId &&
               p.ProductCategoryId == productDetailData.ProductCategoryId &&
               p.Promotion.StartDate.CompareTo(today) <= 0 &&
               p.Promotion.EndDate.Value.CompareTo(today) >= 0 && p.Promotion.IsStopped == false)
               .Select(p => new PromotionModel()
               {
                   Name = p.Promotion.Name,
                   IsPercentDiscount = p.Promotion.IsPercentDiscount,
                   PercentNumber = p.Promotion.PercentNumber,
                   MaximumDiscountAmount = p.Promotion.MaximumDiscountAmount,
               })
               .ToListAsync(cancellationToken: cancellationToken);

            if (productCategoryPromotions.Count > 0)
            {
                ApplyPromotionProductCategory(productCategoryPromotions, productDetailData);
            }

            /// Don't apply promotion specific product if the category has promotion
            if (productPromotions.Count > 0 && productCategoryPromotions.Count == 0)
            {
                ApplyPromotionProduct(productPromotions, productDetailData);
            }

            var productToppings = await _unitOfWork.Products
                .GetAllToppingBelongToProduct(loggedUser.StoreId.Value, productDetailData.Id)
                .AsNoTracking()
                .OrderBy(x => x.CreatedTime)
                .Select(t => new ProductToppingModel()
                {
                    ToppingId = t.Id,
                    Name = t.Name,
                    PriceValue = t.ProductPrices.FirstOrDefault().PriceValue,
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return new GetProductDetailByIdResponse
            {
                ProductDetail = productDetailData,
                ProductToppings = productToppings
            };
        }

        private void ApplyPromotionProduct(List<PromotionModel> promotions, ProductDetailByIdModel product)
        {
            foreach (var promotion in promotions)
            {
                foreach (var price in product.ProductPrices)
                {
                    if (price.IsApplyPromotion)
                    {
                        CalculateDiscount(promotion.IsPercentDiscount,
                            promotion.PercentNumber,
                            promotion.MaximumDiscountAmount,
                            product,
                            price);
                    }
                    else
                    {
                        // If product hasn't promotion
                        // Apply new promotion
                        product.IsHasPromotion = true;

                        price.IsApplyPromotion = true;
                        price.OriginalPrice = price.PriceValue;

                        CalculateDiscount(promotion.IsPercentDiscount,
                            promotion.PercentNumber,
                            promotion.MaximumDiscountAmount,
                            product,
                            price);
                    }
                }
                // Set the discountPrice of this product to compare with another promotion
                // If product has more than 1 product prices, take discount price of first product price
                product.DiscountPrice = product.ProductPrices[0].OriginalPrice - product.ProductPrices[0].PriceValue;
            }
        }

        private void ApplyPromotionProductCategory(List<PromotionModel> promotions, ProductDetailByIdModel product)
        {
            foreach (var promotion in promotions)
            {
                foreach (var price in product.ProductPrices)
                {
                    if (price.IsApplyPromotion)
                    {
                        CalculateDiscount(promotion.IsPercentDiscount,
                            promotion.PercentNumber,
                            promotion.MaximumDiscountAmount,
                            product,
                            price);
                    }
                    else
                    {
                        // If product hasn't promotion
                        // Apply new promotion
                        product.IsHasPromotion = true;
                        product.IsPromotionProductCategory = true;

                        price.IsApplyPromotion = true;
                        price.OriginalPrice = price.PriceValue;

                        CalculateDiscount(promotion.IsPercentDiscount,
                            promotion.PercentNumber,
                            promotion.MaximumDiscountAmount,
                            product,
                            price);
                    }
                }
                // Set the discountPrice of this product to compare with another promotion
                // If product has more than 1 product prices, take discount price of first product price
                product.DiscountPrice = product.ProductPrices[0].OriginalPrice - product.ProductPrices[0].PriceValue;
            }
        }

        private static void CalculateDiscount(
            bool isPercentDiscount,
            decimal percentNumber,
            decimal promotionMaximunDiscount,
            ProductDetailByIdModel product,
            ProductDetailByIdModel.ProductPriceDto price)
        {
            decimal maximunDiscount = promotionMaximunDiscount;
            if (isPercentDiscount)
            {
                decimal discountPrice = percentNumber * (price.OriginalPrice / 100);
                if (discountPrice > maximunDiscount && maximunDiscount > 0)
                {
                    // Check if the discount of this promotion is bigger than the previous one
                    if (maximunDiscount > product.DiscountPrice)
                    {
                        product.IsDiscountPercent = true;
                        product.DiscountValue = percentNumber;
                        price.PriceValue = price.OriginalPrice - maximunDiscount;
                    }
                }
                else
                {
                    // Check if the discount of this promotion is bigger than the previous one
                    if (discountPrice > product.DiscountPrice)
                    {
                        product.IsDiscountPercent = true;
                        product.DiscountValue = percentNumber;
                        price.PriceValue = price.OriginalPrice - discountPrice;
                    }
                }
            }
            else
            {
                // Check if the discount of this promotion is bigger than the previous one
                if (maximunDiscount > product.DiscountPrice)
                {
                    product.IsDiscountPercent = false;
                    product.DiscountValue = maximunDiscount;
                    price.PriceValue = price.OriginalPrice - maximunDiscount;
                }
            }
        }
    }
}

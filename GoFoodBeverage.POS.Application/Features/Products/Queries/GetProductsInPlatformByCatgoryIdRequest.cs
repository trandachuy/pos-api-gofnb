using AutoMapper;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.POS.Models.Promotion;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetProductsInPlatformByCatgoryIdRequest : IRequest<GetProductsInPlatformByCatgoryIdResponse>
    {
        public Guid? ProductCategoryId { get; set; }

        public Guid Platform { get; set; }
    }

    public class GetProductsInPlatformByCatgoryIdResponse
    {
        public IEnumerable<ProductActivatedModel> Products { get; set; }
    }

    public class GetProductsInPlatformByCatgoryIdRequestHandler : IRequestHandler<GetProductsInPlatformByCatgoryIdRequest, GetProductsInPlatformByCatgoryIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetProductsInPlatformByCatgoryIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetProductsInPlatformByCatgoryIdResponse> Handle(GetProductsInPlatformByCatgoryIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime today = _dateTimeService.NowUtc;

            /// Get products list activated by product category of store
            var products = await _unitOfWork.ProductProductCategories
                .GetAll()
                .Where(p => p.StoreId == loggedUser.StoreId && p.ProductCategoryId == request.ProductCategoryId && p.Product.IsActive == true)
                .Include(pc => pc.Product).ThenInclude(pcc => pcc.ProductPrices)
                .Include(pc => pc.Product).ThenInclude(pcc => pcc.ProductOptions).ThenInclude(x => x.Option).ThenInclude(x => x.OptionLevel)
                .OrderBy(p => p.Position)
                .Select(p => p.Product)
                .Where(p => p.StoreId == loggedUser.StoreId && p.StatusId == (int)EnumStatus.Active)
                .AsNoTracking()
                .Select(p => new ProductActivatedModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Thumbnail = p.Thumbnail,
                    IsTopping = p.IsTopping,
                    ProductPrices = p.ProductPrices.OrderBy(x => x.PriceValue).Select(pp => new ProductPriceModel() {
                        Id = pp.Id,
                        PriceName = pp.PriceName,
                        PriceValue = pp.PriceValue,
                    }),
                    ProductOptions = p.ProductOptions.Select(po => new ProductActivatedModel.ProductOptionDto() {
                        Id = po.OptionId,
                        Name = po.Option.Name,
                        OptionLevels = po.Option.OptionLevel.Select(ol => new ProductActivatedModel.ProductOptionDto.OptionLevelDto() { 
                            Id = ol.Id,
                            Name = ol.Name,
                            IsSetDefault = ol.IsSetDefault,
                            OptionId = ol.OptionId,
                        })
                    })
                })
                .ToListAsync(cancellationToken: cancellationToken);

            var productIds = products.Select(x => x.Id).ToList();
            var promotions = await _unitOfWork.PromotionProducts
                .GetAll()
                .AsNoTracking()
                .Include(p => p.Promotion)
                .Where(p => p.StoreId == loggedUser.StoreId &&
                        (products.Select(x => x.Id).Contains(p.ProductId) &&
                        p.Promotion.StoreId == loggedUser.StoreId &&
                        p.Promotion.StartDate.Date.CompareTo(today.Date) <= 0 &&
                        ((p.Promotion.EndDate.Value.Date.CompareTo(today.Date) >= 0 && p.Promotion.EndDate.HasValue) || !p.Promotion.EndDate.HasValue) &&
                        (p.Promotion.IsStopped != true || (p.Promotion.EndDate.Value.Date.CompareTo(today.Date) < 0))))
                .ToListAsync(cancellationToken: cancellationToken);

            var promotionProductCategories = await _unitOfWork.PromotionProductCategories
                .GetAll()
                .AsNoTracking()
                .Include(p => p.Promotion)
                .Where(p => p.StoreId == loggedUser.StoreId && (p.ProductCategoryId == request.ProductCategoryId &&
                            p.Promotion.StoreId == loggedUser.StoreId &&
                            p.Promotion.StartDate.Date.CompareTo(today.Date) <= 0 &&
                            ((p.Promotion.EndDate.Value.Date.CompareTo(today.Date) >= 0 && p.Promotion.EndDate.HasValue) || !p.Promotion.EndDate.HasValue) &&
                            (p.Promotion.IsStopped != true || (p.Promotion.EndDate.Value.Date.CompareTo(today.Date) < 0))))
                .Select(p => new PromotionModel()
                {
                    Name = p.Promotion.Name,
                    IsPercentDiscount = p.Promotion.IsPercentDiscount,
                    PercentNumber = p.Promotion.PercentNumber,
                    MaximumDiscountAmount = p.Promotion.MaximumDiscountAmount,
                })
                .ToListAsync(cancellationToken: cancellationToken);

            // Remove product not in POS platform
            var productPosIds = _unitOfWork.ProductPlatforms
                .Find(p => p.StoreId == loggedUser.StoreId && p.PlatformId == request.Platform && products.Select(x => x.Id).Contains(p.ProductId))
                .Select(x => x.ProductId);

            products = products.Where(p => productPosIds.Contains(p.Id)).ToList();
            if (promotionProductCategories.Count > 0)
            {
                ApplyPromotionProductCategory(promotionProductCategories, products);
            }

            // Don't apply promotion specific product if the category has promotion
            if (promotions.Count > 0 && promotionProductCategories.Count == 0)
            {
                var productsHasPromotion = products.Where(x => promotions.Any(p => p.ProductId == x.Id)).ToList();
                ApplyPromotionProduct(promotions, productsHasPromotion);
            }

            var response = new GetProductsInPlatformByCatgoryIdResponse()
            {
                Products = products
            };

            return response;
        }

        private void ApplyPromotionProduct(IEnumerable<PromotionProduct> promotions, List<ProductActivatedModel> productActivatedModels)
        {
            foreach (var product in productActivatedModels)
            {
                var listPromotionByProducts = promotions.Where(x => x.ProductId == product.Id).ToList();
                foreach (var promotion in listPromotionByProducts)
                {
                    foreach (var price in product.ProductPrices)
                    {
                        if (price.IsApplyPromotion)
                        {
                            CalculateDiscount(promotion.Promotion.IsPercentDiscount, promotion.Promotion.PercentNumber,
                                promotion.Promotion.MaximumDiscountAmount, product, price);
                        }
                        else
                        {
                            // If product hasn't promotion
                            // Apply new promotion
                            product.IsHasPromotion = true;
                            price.IsApplyPromotion = true;

                            price.OriginalPrice = price.PriceValue;

                            CalculateDiscount(promotion.Promotion.IsPercentDiscount, promotion.Promotion.PercentNumber,
                                promotion.Promotion.MaximumDiscountAmount, product, price);
                        }
                    }
                    // Set the discountPrice of this product to compare with another promotion
                    // If product has more than 1 product prices, take discount price of first product price
                    product.DiscountPrice = product.ProductPrices.First().OriginalPrice - product.ProductPrices.First().PriceValue;
                }
            }
        }

        private void ApplyPromotionProductCategory(List<PromotionModel> promotions, List<ProductActivatedModel> productActivatedModels)
        {
            foreach (var product in productActivatedModels)
            {
                foreach (var promotion in promotions)
                {
                    foreach (var price in product.ProductPrices)
                    {
                        if (price.IsApplyPromotion)
                        {
                            CalculateDiscount(promotion.IsPercentDiscount, promotion.PercentNumber,
                                promotion.MaximumDiscountAmount, product, price);
                        }
                        else
                        {
                            // If product hasn't promotion
                            // Apply new promotion
                            product.IsHasPromotion = true;
                            product.IsPromotionProductCategory = true;
                            price.IsApplyPromotion = true;

                            price.OriginalPrice = price.PriceValue;

                            CalculateDiscount(promotion.IsPercentDiscount, promotion.PercentNumber,
                                promotion.MaximumDiscountAmount, product, price);
                        }
                    }
                    // Set the discountPrice of this product to compare with another promotion
                    // If product has more than 1 product prices, take discount price of first product price
                    product.DiscountPrice = product.ProductPrices.First().OriginalPrice - product.ProductPrices.First().PriceValue;
                }
            }
        }

        private void CalculateDiscount(bool isPercentDiscount, decimal percentNumber, decimal promotionMaximunDiscount,
            ProductActivatedModel product, ProductPriceModel price)
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

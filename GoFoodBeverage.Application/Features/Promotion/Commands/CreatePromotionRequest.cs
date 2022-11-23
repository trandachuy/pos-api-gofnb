using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Promotion;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Promotions.Commands
{
    public class CreatePromotionRequest : IRequest<bool>
    {
        public CreateNewPromotionModel Promotion { get; set; }
    }

    public class CreatePromotionRequestHandler : IRequestHandler<CreatePromotionRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreatePromotionRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreatePromotionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var promotionNameExisted = await _unitOfWork.Promotions.GetPromotionByNameInStoreAsync(request.Promotion.Name, loggedUser.StoreId.Value);
            ThrowError.Against(promotionNameExisted != null, "Promotion name has already existed");

            var promotion = CreatePromotion(request, loggedUser.StoreId.Value, loggedUser.AccountId.Value);
            await _unitOfWork.Promotions.AddAsync(promotion);

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(CreatePromotionRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Promotion.Name), "Please enter promotion name");
        }

        private static Promotion CreatePromotion(CreatePromotionRequest request, Guid storeId, Guid accountId)
        {
            var newPromotion = new Promotion()
            {
                Name = request.Promotion.Name,
                PromotionTypeId = request.Promotion.PromotionTypeId,
                IsPercentDiscount = request.Promotion.IsPercentDiscount,
                StoreId = storeId,
                PercentNumber = request.Promotion.PercentNumber,
                MaximumDiscountAmount = request.Promotion.MaximumDiscountAmount,
                StartDate = request.Promotion.StartDate.StartOfDay().ToUniversalTime(),
                TermsAndCondition = request.Promotion.TermsAndCondition,
                IsSpecificBranch = request.Promotion.IsSpecificBranch,
                IsIncludedTopping = request.Promotion.IsIncludedTopping,
                IsStopped = false,
                CreatedUser = accountId
            };

            if (request.Promotion.EndDate.HasValue)
            {
                newPromotion.EndDate = request.Promotion.EndDate.Value.EndOfDay().ToUniversalTime();
            }

            if (request.Promotion.PromotionTypeId == (int)EnumPromotion.DiscountProduct)
            {
                var promotionProducts = new List<PromotionProduct>();
                request.Promotion.ProductIds.ForEach(p =>
                {
                    var promotionProduct = new PromotionProduct()
                    {
                        PromotionId = newPromotion.Id,
                        ProductId = p,
                        CreatedUser = accountId,
                        CreatedTime = DateTime.UtcNow,
                        StoreId = storeId,
                    };
                    promotionProducts.Add(promotionProduct);
                });
                newPromotion.PromotionProducts = promotionProducts;
            }
            else if (request.Promotion.PromotionTypeId == (int)EnumPromotion.DiscountProductCategory)
            {
                var promotionProductCategorys = new List<PromotionProductCategory>();
                request.Promotion.ProductCategoryIds.ForEach(p =>
                {
                    var promotionProductCategory = new PromotionProductCategory()
                    {
                        PromotionId = newPromotion.Id,
                        ProductCategoryId = p,
                        CreatedUser = accountId,
                        CreatedTime = DateTime.UtcNow,
                        StoreId = storeId,
                    };
                    promotionProductCategorys.Add(promotionProductCategory);
                });
                newPromotion.PromotionProductCategories = promotionProductCategorys;
            }
            else
            {
                newPromotion.IsMinimumPurchaseAmount = request.Promotion.IsMinimumPurchaseAmount;
                if (request.Promotion.IsMinimumPurchaseAmount.Value)
                {
                    newPromotion.MinimumPurchaseAmount = request.Promotion.MinimumPurchaseAmount;
                }
            };

            if (request.Promotion.IsSpecificBranch)
            {
                var promotionBranches = new List<PromotionBranch>();
                request.Promotion.BranchIds.ForEach(p =>
                {
                    var promotionBranch = new PromotionBranch()
                    {
                        PromotionId = newPromotion.Id,
                        BranchId = p,
                        CreatedUser = accountId,
                        CreatedTime = DateTime.UtcNow,
                        StoreId = storeId,
                    };
                    promotionBranches.Add(promotionBranch);
                });
                newPromotion.PromotionBranches = promotionBranches;
            }

            return newPromotion;
        }
    }
}

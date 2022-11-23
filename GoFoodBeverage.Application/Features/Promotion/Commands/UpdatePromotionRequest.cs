using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Promotion;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Promotions.Commands
{
    public class UpdatePromotionRequest : IRequest<bool>
    {
        public UpdatePromotionModel Promotion { get; set; }
    }

    public class UpdatePromotionRequestHandler : IRequestHandler<UpdatePromotionRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdatePromotionRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdatePromotionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidationAsync(request);

            var promotion = await _unitOfWork.Promotions
                .GetPromotionByIdInStoreAsync(request.Promotion.Id, loggedUser.StoreId.Value);

            var promotionUpdate = await UpdatePromotion(promotion, request, loggedUser.AccountId.Value);
            
            await _unitOfWork.Promotions.UpdateAsync(promotionUpdate);

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidationAsync(UpdatePromotionRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Promotion.Name), "Please enter promotion name");
            ThrowError.BadRequestAgainstNull(request.Promotion.PromotionTypeId, "Please select promotion type");
        }

        public async Task<Promotion> UpdatePromotion(Promotion promotion, UpdatePromotionRequest request, Guid accountId)
        {
            promotion.Name = request.Promotion.Name;
            promotion.PromotionTypeId = request.Promotion.PromotionTypeId;
            promotion.IsPercentDiscount = request.Promotion.IsPercentDiscount;
            promotion.PercentNumber = request.Promotion.PercentNumber;
            promotion.MaximumDiscountAmount = request.Promotion.MaximumDiscountAmount;
            promotion.StartDate = request.Promotion.StartDate.StartOfDay().ToUniversalTime();
            promotion.EndDate = request.Promotion.EndDate?.EndOfDay().ToUniversalTime();
            promotion.TermsAndCondition = request.Promotion.TermsAndCondition;
            promotion.LastSavedUser = accountId;
            promotion.IsIncludedTopping = request.Promotion.IsIncludedTopping;
            promotion.IsMinimumPurchaseAmount = request.Promotion.IsMinimumPurchaseAmount;
            promotion.IsSpecificBranch = request.Promotion.IsSpecificBranch;
            promotion.MinimumPurchaseAmount = request.Promotion.MinimumPurchaseAmount;

            //Handle Branches
            if (request.Promotion.IsSpecificBranch)
            {
                var currentPromotionBranch = promotion.PromotionBranches.ToList();
                var newPromotionBranch = new List<PromotionBranch>();

                if (request.Promotion.BranchIds != null && request.Promotion.BranchIds.Any())
                {
                    //Delete
                    var deleteItems = currentPromotionBranch
                        .Where(x => !request.Promotion.BranchIds.Contains(x.BranchId));

                    await _unitOfWork.PromotionBranches.RemoveRangeAsync(deleteItems);

                    //AddNew
                    request.Promotion.BranchIds.ForEach(branchId =>
                    {
                        var promotionBranch = currentPromotionBranch
                            .FirstOrDefault(p => p.BranchId == branchId);
                        if (promotionBranch == null)
                        {
                            var newBranch = new PromotionBranch()
                            {
                                PromotionId = request.Promotion.Id,
                                BranchId = branchId,
                                CreatedUser = accountId,
                                LastSavedUser = accountId,
                                StoreId = promotion.StoreId
                            };
                            newPromotionBranch.Add(newBranch);
                        }
                    });

                    await _unitOfWork.PromotionBranches.AddRangeAsync(newPromotionBranch);
                }
                else
                {
                    await _unitOfWork.PromotionBranches.RemoveRangeAsync(currentPromotionBranch);
                }
            }
            
            //Handle PromotionType
            if (request.Promotion.PromotionTypeId == (int)EnumPromotion.DiscountProduct)
            {
                var currentPromotionProduct = promotion.PromotionProducts.ToList();
                var newPromotionProduct = new List<PromotionProduct>();

                if (request.Promotion.ProductIds != null && request.Promotion.ProductIds.Any())
                {
                    //Delete
                    var deleteItems = currentPromotionProduct
                        .Where(x => !request.Promotion.ProductIds.Contains(x.ProductId));

                    await _unitOfWork.PromotionProducts.RemoveRangeAsync(deleteItems);

                    //AddNew
                    request.Promotion.ProductIds.ForEach(productId =>
                    {
                        var promotionProduct = currentPromotionProduct
                            .FirstOrDefault(p => p.ProductId == productId);
                        if (promotionProduct == null)
                        {
                            var newProduct = new PromotionProduct()
                            {
                                PromotionId = request.Promotion.Id,
                                ProductId = productId,
                                CreatedUser = accountId,
                                LastSavedUser = accountId,
                                StoreId = promotion.StoreId
                            };
                            newPromotionProduct.Add(newProduct);
                        }
                    });

                    await _unitOfWork.PromotionProducts.AddRangeAsync(newPromotionProduct);
                }
                else
                {
                    await _unitOfWork.PromotionProducts.RemoveRangeAsync(currentPromotionProduct);
                }
                
            }
            else if (request.Promotion.PromotionTypeId == (int)EnumPromotion.DiscountProductCategory)
            {
                var currentPromotionProductCategory = promotion.PromotionProductCategories.ToList();
                var newPromotionProductCategory = new List<PromotionProductCategory>();

                if (request.Promotion.ProductCategoryIds != null && request.Promotion.ProductCategoryIds.Any())
                {
                    //Delete
                    var deleteItems = currentPromotionProductCategory
                        .Where(x => !request.Promotion.ProductCategoryIds.Contains(x.ProductCategoryId));

                    await _unitOfWork.PromotionProductCategories.RemoveRangeAsync(deleteItems);

                    //AddNew
                    request.Promotion.ProductCategoryIds.ForEach(productCategoryId =>
                    {
                        var promotionProductCategory = currentPromotionProductCategory
                            .FirstOrDefault(pc => pc.ProductCategoryId == productCategoryId);
                        if (promotionProductCategory == null)
                        {
                            var newProductCategory = new PromotionProductCategory()
                            {
                                PromotionId = request.Promotion.Id,
                                ProductCategoryId = productCategoryId,
                                CreatedUser = accountId,
                                LastSavedUser = accountId,
                                StoreId = promotion.StoreId
                            };
                            newPromotionProductCategory.Add(newProductCategory);
                        }
                    });

                    await _unitOfWork.PromotionProductCategories.AddRangeAsync(newPromotionProductCategory);
                }
                else
                {
                    await _unitOfWork.PromotionProductCategories.RemoveRangeAsync(currentPromotionProductCategory);
                }
            }

            return promotion;
        }
    }
}

using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class UpdateComboRequest : IRequest<bool>
    {
        public UpdateComboRequestModel Combo { get; set; }
    }

    public class UpdateComboRequestHandler : IRequestHandler<UpdateComboRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateComboRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateComboRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            var combo = await _unitOfWork.Combos
                .GetComboByIdAsync(request.Combo.ComboId, loggedUser.StoreId);

            ThrowError.Against(combo == null, "Cannot find combo information");

            var combos = await _unitOfWork.Combos
                    .GetAllCombosInStoreActivies(loggedUser.StoreId.Value)
                    .Where(g => g.Id != request.Combo.ComboId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

            var comboNameExisted = combos.Find(i => i.Name == request.Combo.ComboName);
            ThrowError.Against(comboNameExisted != null, "Name of combo has already existed");

            var staff = await _unitOfWork.Staffs.GetStaffByIdAsync(loggedUser.Id.Value);
            ThrowError.Against(staff == null, "Cannot find staff information, please authenticate before!");

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);
            ThrowError.Against(store == null, "Cannot find store information, please authenticate before!");

            ThrowError.Against(request.Combo.IsShowAllBranches == false && (request.Combo.BranchIds == null || request.Combo.BranchIds?.Count() == 0), "Please select branch");

            var branches = new List<StoreBranch>();
            if (request.Combo.IsShowAllBranches)
            {
                branches = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(loggedUser.StoreId.Value)
                .ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                branches = await _unitOfWork.StoreBranches
                .GetAnyStoreBranchByIdAsync(request.Combo.BranchIds)
                .ToListAsync(cancellationToken: cancellationToken);
            }

            var modifiedCombo = await UpdateComboAsync(combo, request, branches, loggedUser.AccountId.Value);
            await _unitOfWork.Combos.UpdateAsync(modifiedCombo);

            /// UPDATE ComboPricing
            await UpdateComboPricingsAsync(combo, request, loggedUser.AccountId.Value);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Combo,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedCombo.Id,
                ObjectName = modifiedCombo.Name.ToString(),
                ObjectThumbnail = modifiedCombo.Thumbnail
            });

            return true;
        }

        private static void RequestValidation(UpdateComboRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Combo.ComboName), "Please enter name combo.");
        }

        public async Task UpdateComboPricingsAsync(Combo combo, UpdateComboRequest request, Guid? loggedUserId)
        {
            if (request.Combo.ComboPricings == null || !request.Combo.ComboPricings.Any()) return;

            //DELETE ALL ComboPricing Table and ComboPricingProductPrice Table By ComboId
            if (request.Combo.ComboTypeId == EnumComboType.Specific)
            {
                _unitOfWork.ComboPricings.RemoveRange(combo.ComboPricings);
            }
            else
            {
                _unitOfWork.ComboPricings.RemoveRange(combo.ComboPricings);
                var comboPricings = new List<ComboPricing>();
                foreach (var pricing in request.Combo.ComboPricings)
                {
                    var originalPrice = pricing.ComboPricingProducts.Sum(i => i.ProductPrice);
                    decimal? sellingPrice = 0;
                    switch (request.Combo.ComboPriceTypeId)
                    {
                        case EnumComboPriceType.Fixed:
                            sellingPrice = request.Combo.SellingPrice;
                            break;
                        case EnumComboPriceType.Specific:
                            sellingPrice = pricing.SellingPrice;
                            break;
                        default:
                            break;
                    }

                    /// If the price is not changed, default will take the total price of the product
                    if (sellingPrice == 0)
                    {
                        sellingPrice = pricing.ComboPricingProducts.Sum((p) => p.ProductPrice);
                    }

                    var comboPricing = new ComboPricing()
                    {
                        ComboId = request.Combo.ComboId,
                        ComboName = pricing.ComboProductName,
                        OriginalPrice = originalPrice,
                        SellingPrice = sellingPrice,
                        CreatedUser = loggedUserId,
                        LastSavedUser = loggedUserId,
                        StoreId = combo.StoreId,
                        ComboPricingProducts = new List<ComboPricingProductPrice>()
                    };

                    foreach (var comboPricingProduct in pricing.ComboPricingProducts)
                    {
                        comboPricing.ComboPricingProducts.Add(new ComboPricingProductPrice()
                        {
                            ProductPriceId = comboPricingProduct.ProductPriceId,
                            SellingPrice = comboPricingProduct.ProductPrice,
                            CreatedUser = loggedUserId,
                            LastSavedUser = loggedUserId,
                            StoreId = combo.StoreId,
                        });
                    }
                    comboPricings.Add(comboPricing);
                }

                _unitOfWork.ComboPricings.AddRange(comboPricings);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Combo> UpdateComboAsync(Combo combo, UpdateComboRequest request, List<StoreBranch> branches, Guid accountId)
        {
            combo.Name = request.Combo.ComboName;
            combo.Description = request.Combo.Description;
            combo.Thumbnail = request.Combo.Thumbnail;
            combo.IsShowAllBranches = request.Combo.IsShowAllBranches;
            combo.ComboTypeId = request.Combo.ComboTypeId;
            combo.ComboPriceTypeId = request.Combo.ComboPriceTypeId;
            combo.SellingPrice = request?.Combo?.SellingPrice;
            combo.LastSavedUser = accountId;
            combo.LastSavedTime = DateTime.UtcNow;
            combo.StartDate = request.Combo.StartDate;
            combo.EndDate = request?.Combo.EndDate;

            var currentStoreBranches = combo.ComboStoreBranches.ToList();
            var newStoreBranches = new List<ComboStoreBranch>();

            if (request.Combo.IsShowAllBranches)
            {
                _unitOfWork.ComboStoreBranches.RemoveRange(currentStoreBranches);
            }
            else
            {
                var listDeleteStoreBranches = currentStoreBranches.Where(p => !request.Combo.BranchIds.Select(i => i).Contains(p.BranchId)).ToList();
                _unitOfWork.ComboStoreBranches.RemoveRange(listDeleteStoreBranches);

                branches.ForEach(branch =>
                {
                    var branchItem = currentStoreBranches.FirstOrDefault(p => p.BranchId == branch.Id);
                    if (branchItem == null)
                    {
                        var comboStoreBranch = new ComboStoreBranch()
                        {
                            ComboId = combo.Id,
                            BranchId = branch.Id,
                            CreatedUser = accountId,
                            StoreId = combo.StoreId,
                        };
                        newStoreBranches.Add(comboStoreBranch);
                    }
                });

                _unitOfWork.ComboStoreBranches.AddRange(newStoreBranches);
            }

            switch (request.Combo.ComboTypeId)
            {
                case EnumComboType.Flexible:
                    await UpdateComboProductPriceGroups(combo, request, accountId);
                    break;
                case EnumComboType.Specific:
                    await UpdateComboProductPricesAsync(combo, request, accountId);
                    break;
                default:
                    break;
            }

            return combo;
        }

        private async Task UpdateComboProductPricesAsync(Combo combo, UpdateComboRequest request, Guid accountId)
        {
            if (request.Combo.ProductPriceIds == null || !request.Combo.ProductPriceIds.Any()) return;

            var productPrices = await _unitOfWork.ProductPrices
                .Find(p => p.StoreId == combo.StoreId && request.Combo.ProductPriceIds.Any(pid => pid == p.Id))
                .ToListAsync();

            var currentComboProductPrices = combo.ComboProductPrices.ToList();
            var newComboProductPrices = new List<ComboProductPrice>();
            var listDeleteComboProductPrices = currentComboProductPrices.Where(p => !request.Combo.ProductPriceIds.Select(i => i).Contains(p.ProductPriceId));
            productPrices.ForEach(productPrice =>
            {
                var productPriceItem = currentComboProductPrices.FirstOrDefault(p => p.ProductPriceId == productPrice.Id);
                if (productPriceItem == null)
                {
                    var comboProductPrice = new ComboProductPrice()
                    {
                        ComboId = combo.Id,
                        ProductPriceId = productPrice.Id,
                        PriceValue = productPrice.PriceValue,
                        CreatedUser = accountId,
                        LastSavedUser = accountId,
                        StoreId = combo.StoreId,
                    };
                    newComboProductPrices.Add(comboProductPrice);
                }
            });

            var currentComboProductGroups = combo.ComboProductGroups.ToList();
            if (currentComboProductGroups.Count() > 0)
            {
                foreach (var currentComboProductGroup in currentComboProductGroups)
                {
                    await _unitOfWork.ComboProductGroupProductPrices.RemoveRangeAsync(currentComboProductGroup.ComboProductGroupProductPrices);
                }
                await _unitOfWork.ComboProductGroups.RemoveRangeAsync(currentComboProductGroups);
            }

            await _unitOfWork.ComboProductPrices.RemoveRangeAsync(listDeleteComboProductPrices);
            await _unitOfWork.ComboProductPrices.AddRangeAsync(newComboProductPrices);
        }

        private async Task UpdateComboProductPriceGroups(Combo combo, UpdateComboRequest request, Guid accountId)
        {
            ThrowError.Against(request.Combo.ProductGroups == null || request.Combo.ProductGroups.Count() <= 1, "Please select more product group");

            var currentComboProductGroups = combo.ComboProductGroups.ToList();
            var newComboProductGroups = new List<ComboProductGroup>();
            var updateComboProductGroups = new List<ComboProductGroup>();
            var listDeleteComboProductGroups = currentComboProductGroups.Where(p => !request.Combo.ProductGroups.Select(i => i.Id).Contains(p.Id));

            foreach (var productGroup in request.Combo.ProductGroups)
            {
                var comboProductGroupExisted = currentComboProductGroups.FirstOrDefault(x => x.Id == productGroup.Id);
                var currentComboProductCategoryProductPrices = productGroup.ProductPriceIds.ToList();
                if (comboProductGroupExisted == null)
                {
                    var newComboProductGroup = new ComboProductGroup()
                    {
                        ComboId = combo.Id,
                        ProductCategoryId = productGroup.ProductCategoryId,
                        Quantity = productGroup.Quantity,
                        CreatedUser = accountId,
                        StoreId = combo.StoreId,
                        ComboProductGroupProductPrices = new List<ComboProductGroupProductPrice>()
                    };

                    var newComboProductGroupProductPrices = new List<ComboProductGroupProductPrice>();
                    foreach (var productPriceId in productGroup.ProductPriceIds)
                    {
                        var comboProductGroupProductPrices = new ComboProductGroupProductPrice()
                        {
                            ProductPriceId = productPriceId,
                            CreatedUser = accountId,
                            StoreId = combo.StoreId,
                        };
                        newComboProductGroup.ComboProductGroupProductPrices.Add(comboProductGroupProductPrices);
                    }

                    newComboProductGroups.Add(newComboProductGroup);
                } else
                {
                    comboProductGroupExisted.ProductCategoryId = productGroup.ProductCategoryId;
                    comboProductGroupExisted.Quantity = productGroup.Quantity;
                    comboProductGroupExisted.LastSavedUser = accountId;

                    var productPrices = await _unitOfWork.ProductPrices
                                            .Find(p => p.StoreId == combo.StoreId && productGroup.ProductPriceIds.Any(pid => pid == p.Id))
                                            .ToListAsync();
                    var listDeleteComboProductCategoryProductPrices = comboProductGroupExisted.ComboProductGroupProductPrices.Where(p => !productPrices.Select(i => i.Id).Contains(p.ProductPriceId));

                    var newComboProductCategoryProductsPrices = new List<ComboProductGroupProductPrice>();
                    productPrices.ForEach(productPrice =>
                    {
                        var productPriceItem = comboProductGroupExisted.ComboProductGroupProductPrices.FirstOrDefault(p => p.ProductPriceId == productPrice.Id);
                        if (productPriceItem == null)
                        {
                            var comboProductCategoryProductPrice = new ComboProductGroupProductPrice()
                            {
                                ComboProductGroupId = comboProductGroupExisted.Id,
                                ProductPriceId = productPrice.Id,
                                LastSavedUser = accountId,
                                StoreId = combo.StoreId,
                            };
                            newComboProductCategoryProductsPrices.Add(comboProductCategoryProductPrice);
                        }
                    });

                    if (newComboProductCategoryProductsPrices.Any())
                    {
                        _unitOfWork.ComboProductGroupProductPrices.AddRange(newComboProductCategoryProductsPrices);
                    }

                    if (listDeleteComboProductCategoryProductPrices.Any())
                    {
                        _unitOfWork.ComboProductGroupProductPrices.RemoveRange(listDeleteComboProductCategoryProductPrices);
                    }
                }
            }


            var currentComboProductPrices = combo.ComboProductPrices.ToList();
            if (currentComboProductPrices.Count() > 0)
            {
                _unitOfWork.ComboProductPrices.RemoveRange(currentComboProductPrices);
            }

            if(listDeleteComboProductGroups.Any())
            {
                _unitOfWork.ComboProductGroups.RemoveRange(listDeleteComboProductGroups);
            }

            if (newComboProductGroups.Any())
            {
                _unitOfWork.ComboProductGroups.AddRange(newComboProductGroups);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

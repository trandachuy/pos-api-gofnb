using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class CreateComboRequest : IRequest<bool>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public bool IsShowAllBranches { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType PriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public IEnumerable<Guid> BranchIds { get; set; }

        public IEnumerable<Guid> ProductPriceIds { get; set; }

        public IEnumerable<ProductGroupDto> ProductGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public class ProductGroupDto
        {
            /// <summary>
            /// Product category id
            /// </summary>
            public Guid CategoryId { get; set; }

            /// <summary>
            /// Minimum number of item
            /// </summary>
            public int Quantity { get; set; }

            public IEnumerable<Guid> ProductPriceIds { get; set; }
        }

        /// <summary>
        /// List product combo
        /// </summary>
        public IEnumerable<ComboPricingDto> ComboPricings { get; set; }


        public class ComboPricingDto
        {
            public string ComboProductName { get; set; }

            /// <summary>
            /// Selling price get from UI
            /// </summary>
            public decimal? SellingPrice { get; set; }

            /// <summary>
            /// Product list of combo
            /// </summary>
            public IEnumerable<ComboPricingProductDto> ComboPricingProducts { get; set; }
        }

        public class ComboPricingProductDto
        {
            public Guid ProductPriceId { get; set; }

            /// <summary>
            /// Price of product selected to apply to create combo
            /// </summary>
            public decimal? ProductPrice { get; set; }
        }
    }

    public class CreateComboRequestHandler : IRequestHandler<CreateComboRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateComboRequestHandler(
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

        public async Task<bool> Handle(CreateComboRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            await RequestValidationAsync(request, cancellationToken);

            var newCombo = await CreateComboAsync(request, loggedUser.AccountId.Value, loggedUser.StoreId);
            await _unitOfWork.Combos.AddAsync(newCombo);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Combo,
                ActionType = EnumActionType.Created,
                ObjectId = newCombo.Id,
                ObjectName = newCombo.Name.ToString(),
                ObjectThumbnail = newCombo.Thumbnail
            });

            return true;
        }

        private async Task RequestValidationAsync(CreateComboRequest request, CancellationToken cancellationToken)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter combo name");

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var comboNameExisted = await _unitOfWork.Combos
                .GetAllCombosInStoreActivies(loggedUser.StoreId.Value)
                .Where(c => c.Name.ToLower().Equals(request.Name.ToLower()))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(comboNameExisted != null, "The combo name existed");

            ThrowError.Against(request.IsShowAllBranches == false && (request.BranchIds == null || request.BranchIds?.Count() == 0), "Please select branch");
        }

        /// <summary>
        /// This is method create a combo entity class from create combo reuqest model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accountId"></param>
        /// <returns>Combo entity class</returns>
        private async Task<Combo> CreateComboAsync(CreateComboRequest request, Guid accountId, Guid? storeId)
        {
            var combo = new Combo()
            {
                StoreId = storeId,
                Name = request.Name,
                Description = request.Description,
                Thumbnail = request.Thumbnail,
                IsShowAllBranches = request.IsShowAllBranches,
                ComboTypeId = request.ComboTypeId,
                ComboPriceTypeId = request.PriceTypeId,
                SellingPrice = request.SellingPrice,
                CreatedUser = accountId,
                LastSavedUser = accountId,
                ComboStoreBranches = new List<ComboStoreBranch>(),
                ComboProductGroups = new List<ComboProductGroup>(),
                ComboProductPrices = new List<ComboProductPrice>(),
                ComboPricings = new List<ComboPricing>(),
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            /// Create combo apply to branches
            if (request.IsShowAllBranches == false && request.BranchIds != null && request.BranchIds.Any())
            {
                foreach (var branchId in request.BranchIds)
                {
                    combo.ComboStoreBranches.Add(new ComboStoreBranch()
                    {
                        BranchId = branchId,
                        StoreId = storeId,
                        CreatedUser = accountId,
                        LastSavedUser = accountId,
                    });
                }
            }

            switch (request.ComboTypeId)
            {
                case EnumComboType.Flexible:
                    CreateComboProductPriceGroups(request, combo, accountId, storeId);
                    await CreateComboPricingsAsync(request, combo, accountId, storeId);
                    break;
                case EnumComboType.Specific:
                    await CreateComboProductPricesAsync(request, combo, accountId, storeId);
                    break;
                default:
                    break;
            }

            return combo;
        }

        private static void CreateComboProductPriceGroups(CreateComboRequest request, Combo combo, Guid accountId, Guid? storeId)
        {
            ThrowError.Against(request.ProductGroups == null || request.ProductGroups.Count() <= 1, "Please select more product group");

            foreach (var productGroup in request.ProductGroups)
            {
                var newComboProductGroup = new ComboProductGroup()
                {
                    ProductCategoryId = productGroup.CategoryId,
                    Quantity = productGroup.Quantity,
                    CreatedUser = accountId,
                    LastSavedUser = accountId,
                    StoreId = storeId,
                    ComboProductGroupProductPrices = new List<ComboProductGroupProductPrice>()
                };

                foreach (var productPriceId in productGroup.ProductPriceIds)
                {
                    newComboProductGroup.ComboProductGroupProductPrices.Add(new ComboProductGroupProductPrice()
                    {
                        ProductPriceId = productPriceId,
                        CreatedUser = accountId,
                        LastSavedUser = accountId,
                        StoreId = storeId,
                    });
                }

                combo.ComboProductGroups.Add(newComboProductGroup);
            }
        }

        private async Task CreateComboProductPricesAsync(CreateComboRequest request, Combo combo, Guid accountId, Guid? storeId)
        {
            if (request.ProductPriceIds == null || !request.ProductPriceIds.Any()) return;

            var productPrices = await _unitOfWork.ProductPrices
                .Find(p => p.StoreId == storeId && request.ProductPriceIds.Any(pid => pid == p.Id))
                .AsNoTracking()
                .ToListAsync();
            foreach (var productPriceId in request.ProductPriceIds)
            {
                var productPrice = productPrices.FirstOrDefault(p => p.Id == productPriceId);
                ThrowError.Against(productPrice == null, "Can not find product price");

                combo.ComboProductPrices.Add(new ComboProductPrice()
                {
                    ProductPriceId = productPrice.Id,
                    PriceValue = productPrice.PriceValue,
                    CreatedUser = accountId,
                    LastSavedUser = accountId,
                    StoreId = storeId,
                });
            }
        }

        /// <summary>
        /// Create combo - price.
        /// If the combo price type is fixed => all combos selling price will be get from input and SAME the price.
        /// If the combo price type is Specific => combo selling price will be get from input.
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="request"></param>
        private async Task CreateComboPricingsAsync(CreateComboRequest request, Combo combo, Guid? loggedUserId, Guid? storeId)
        {
            if (request.ComboPricings == null || !request.ComboPricings.Any()) return;

            var productPriceIds = request.ComboPricings.SelectMany(c => c.ComboPricingProducts.Select(p => p.ProductPriceId));
            var productPrices = await _unitOfWork.ProductPrices
                .Find(p => p.StoreId == storeId && productPriceIds.Any(pid => pid == p.Id))
                .Include(p => p.Product)
                .ToListAsync();

            foreach (var pricing in request.ComboPricings)
            {
                var originalPrice = pricing.ComboPricingProducts.Sum(i => i.ProductPrice);
                decimal? sellingPrice = 0;
                switch (request.PriceTypeId)
                {
                    case EnumComboPriceType.Fixed:
                        sellingPrice = request.SellingPrice;
                        break;
                    case EnumComboPriceType.Specific:
                        sellingPrice = pricing.SellingPrice;
                        break;
                    default:
                        break;
                }

                //If the price is not changed, default will take the total price of the product
                if (sellingPrice == 0)
                {
                    sellingPrice = pricing.ComboPricingProducts.Sum((p) => p.ProductPrice);
                }

                var comboPricing = new ComboPricing()
                {
                    ComboName = pricing.ComboProductName,
                    OriginalPrice = originalPrice,
                    SellingPrice = sellingPrice,
                    CreatedUser = loggedUserId,
                    LastSavedUser = loggedUserId,
                    StoreId = storeId,
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
                        StoreId = storeId,
                    });
                }

                combo.ComboPricings.Add(comboPricing);
            }
        }
    }
}

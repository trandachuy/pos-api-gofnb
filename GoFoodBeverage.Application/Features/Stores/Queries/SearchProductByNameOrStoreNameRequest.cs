using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;

using GoogleServices.Distance;

using MediatR;

using Microsoft.EntityFrameworkCore;

using MoreLinq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static GoFoodBeverage.Application.Features.Stores.Queries.SearchProductByNameOrStoreNameResponse;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class SearchProductByNameOrStoreNameRequest : IRequest<SearchProductByNameOrStoreNameResponse>
    {
        public double Lat { get; set; }

        public double Lng { get; set; }

        public string KeySearch { get; set; }

        public Guid? AccountId { get; set; }
    }

    public class SearchProductByNameOrStoreNameResponse
    {
        public List<SearchItemResult> SearchItemResults { get; set; } = new List<SearchItemResult>();

        public class SearchItemResult
        {
            public Guid StoreId { get; set; }

            public Guid StoreBrandId { get; set; }

            public string Name { get; set; }

            public int MatchFromIndex { get; set; } = 0;

            public int MatchToIndex { get; set; } = 0;

            public bool IsPromo { get; set; }

            public string Thumbnail { get; set; }

            public double Rating { get; set; } = 0.0;

            public double Distance { get; set; }

            public string CurrencySymbol { get; set; }

            public List<ProductItemResult> Products { get; set; } = new List<ProductItemResult>();
        }

        public class ProductItemResult
        {
            public Guid Id { get; set; } //ComboId or ProductId

            public string Name { get; set; }

            public int MatchFromIndex { get; set; } = 0;

            public int MatchToIndex { get; set; } = 0;

            public string Thumbnail { get; set; }

            public decimal Price { get; set; }

            public decimal? SellingPrice { get; set; }

            public bool HasDiscount { get; set; }
        }
    }

    public class SearchProductByNameOrStoreNameResponseHandler : IRequestHandler<SearchProductByNameOrStoreNameRequest, SearchProductByNameOrStoreNameResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDistanceService _googleDistanceService;

        public SearchProductByNameOrStoreNameResponseHandler(IUnitOfWork unitOfWork, IGoogleDistanceService googleDistanceService)
        {
            _unitOfWork = unitOfWork;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<SearchProductByNameOrStoreNameResponse> Handle(SearchProductByNameOrStoreNameRequest request, CancellationToken cancellationToken)
        {
            var response = new SearchProductByNameOrStoreNameResponse();
            var today = DateTime.UtcNow.Date;

            if (request.AccountId.HasValue)
            {
                var accountSearchHistory = new AccountSearchHistory
                {
                    AccountId = request.AccountId.Value,
                    KeySearch = request.KeySearch,
                };
                await _unitOfWork.AccountSearchHistories.AddAsync(accountSearchHistory);
            }

            #region Step 1: Get all distinct StoreBranches with the closest location with user's location

            var allStoreBranchesByAddress = await _unitOfWork.StoreBranches.GetAllStoreBranches()
            .Include(x => x.Store)
            .ThenInclude(x => x.Currency)
            .Include(x => x.Address)
            .ToListAsync(cancellationToken);

            var storeBranchesSelected = new List<Tuple<StoreBranch, double>>();// <StoreBranch, Distance>

            // Get all Store Branches in distance range (defatul 20km)
            foreach (var aBranch in allStoreBranchesByAddress)
            {
                var address = aBranch.Address;
                if (address.Latitude.HasValue && address.Longitude.HasValue)
                {
                    var distance = MathHelpers.CalculateDistanceBetweenTwoPoints(request.Lat, request.Lng, address.Latitude.Value, address.Longitude.Value);

                    if (distance <= DefaultConstants.DEFAULT_USER_STORE_DISTANCE)
                    {
                        var existsStoreBranch = storeBranchesSelected.FirstOrDefault(x => x.Item1.Store.Id == aBranch.Store.Id);
                        if (existsStoreBranch == null)
                        {
                            storeBranchesSelected.Add(Tuple.Create(aBranch, distance));
                            continue;
                        }
                        if (distance < existsStoreBranch.Item2)
                        {
                            storeBranchesSelected.Remove(existsStoreBranch);
                            storeBranchesSelected.Add(Tuple.Create(aBranch, distance));
                            continue;
                        }
                    }
                }
            }

            #endregion Step 1: Get all distinct StoreBranches with the closest location with user's location

            #region Step 2: From StoreBranches, filter Combo->ProductName match keySearch

            if (storeBranchesSelected.Any())
            {
                var stores = storeBranchesSelected.Select(x => x.Item1.Store).ToList();
                var storesIds = stores.Select(x => x.Id).ToList();

                // Get all Promotion today of Stores
                var promotions = await _unitOfWork.Promotions.GetAll()
                    .Where(p => storesIds.Contains(p.StoreId)
                                        && p.IsStopped == false
                                        && ((p.EndDate == null || (p.EndDate != null && p.EndDate >= DateTime.UtcNow.Date))))
                    .ToListAsync(cancellationToken);

                foreach (var store in stores)
                {


                    // Get Store infomation
                    var storeBranch = storeBranchesSelected.FirstOrDefault(x => x.Item1.Store.Id == store.Id);
                    var matchStoreName = GetIndexKeySearchMatch(store.Title, request.KeySearch);
                    var distance = await _googleDistanceService.GetDistanceBetweenPointsAsync(
                        request.Lat,
                        request.Lng,
                        storeBranch.Item1.Address.Latitude.Value,
                        storeBranch.Item1.Address.Longitude.Value,
                        cancellationToken);
                    var searchItemResult = new SearchItemResult
                    {
                        StoreId = store.Id,
                        Name = store.Title,
                        StoreBrandId = storeBranch.Item1.Id,
                        Thumbnail = store.Logo,
                        MatchFromIndex = matchStoreName.Item1,
                        MatchToIndex = matchStoreName.Item2,
                        IsPromo = promotions.Any(x => x.StoreId == store.Id),
                        CurrencySymbol = store.Currency.Symbol,
                        Distance = distance
                    };

                    var searchProductItemResult = new List<ProductItemResult>();

                    #region Get total 3 Item with priority (Combo -> Product)
                    // Ex: 3combo-0product | 2combo-1product | 1combo-2product | 0combo-3product

                    // Get 3 Combos of stores match keySearch
                    var combos = _unitOfWork.Combos.GetAllCombosInStoreActivies(store.Id)
                        .Include(cp => cp.ComboPricings)
                        .Include(cpp => cpp.ComboProductPrices)
                        .ToList();
                    var combosMatch = combos.Where(
                        x => StringHelpers.RemoveSign4VietnameseString(x.Name).ToLower().Contains(StringHelpers.RemoveSign4VietnameseString(request.KeySearch).ToLower()))
                        .Take(SortConstants.THREE_RECORD)
                        .ToList();
                    if (combosMatch.Any())
                    {
                        foreach (var combo in combosMatch)
                        {
                            decimal originalPrice = 0;
                            decimal sellingPrice = 0;

                            if (combo.ComboTypeId == EnumComboType.Flexible)
                            {
                                var comboPrice = combo.ComboPricings.FirstOrDefault();
                                originalPrice = comboPrice.OriginalPrice != null ? comboPrice.OriginalPrice.Value : 0;
                                sellingPrice = comboPrice.SellingPrice != null ? comboPrice.SellingPrice.Value : 0;
                            }
                            else
                            {
                                var comboProductPrice = combo.ComboProductPrices.FirstOrDefault();
                                originalPrice = comboProductPrice.PriceValue;
                                sellingPrice = combo.SellingPrice != null ? combo.SellingPrice.Value : 0;
                            }

                            var matchProductName = GetIndexKeySearchMatch(combo.Name, request.KeySearch);
                            var searchProductItem = new ProductItemResult
                            {
                                Id = combo.Id,
                                Name = combo.Name,
                                MatchFromIndex = matchProductName.Item1,
                                MatchToIndex = matchProductName.Item2,
                                Thumbnail = combo.Thumbnail ?? string.Empty,
                                Price = originalPrice,
                                SellingPrice = sellingPrice,
                                HasDiscount = true,
                            };
                            searchProductItemResult.Add(searchProductItem);
                        }
                    }

                    // Contitnue get Products if not enough 3 combos
                    if (!combosMatch.Any() || combosMatch?.Count < 3)
                    {
                        int remainProducts = combosMatch == null ? 3 : 3 - combosMatch.Count;
                        // Get all Products in Store
                        var products = _unitOfWork.Products.GetAllProductInStoreActive(store.Id).Include(x => x.ProductPrices).ToList();
                        var productsMatch = products.Where(x =>
                                StringHelpers.RemoveSign4VietnameseString(x.Name).ToLower().Contains(StringHelpers.RemoveSign4VietnameseString(request.KeySearch).ToLower())
                                && x.IsTopping == false)
                            .Take(remainProducts)
                            .ToList();

                        if (productsMatch.Any())
                        {
                            var productMatchIds = productsMatch.Select(x => x.Id).ToList();
                            var promotionProducts = await _unitOfWork.PromotionProducts.GetAll()
                                .Where(x => productMatchIds.Contains(x.ProductId))
                                .Include(x => x.Promotion)
                                .ThenInclude(x => x.PromotionBranches)
                                .Where(x => x.Promotion.PromotionTypeId == (int)EnumPromotion.DiscountProduct)
                                .ToListAsync(cancellationToken);
                            foreach (var product in productsMatch)
                            {

                                var matchProductName = GetIndexKeySearchMatch(product.Name, request.KeySearch);
                                var promotion = promotionProducts?.FirstOrDefault(x => x.ProductId == product.Id)?.Promotion;
                                var searchProductItem = new ProductItemResult
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    MatchFromIndex = matchProductName.Item1,
                                    MatchToIndex = matchProductName.Item2,
                                    Thumbnail = product.Thumbnail ?? string.Empty,
                                    Price = product.ProductPrices.FirstOrDefault().PriceValue,
                                    SellingPrice = promotion == null ? 0 : CalculateSellingPrice(product.ProductPrices.FirstOrDefault().PriceValue, promotion),
                                    HasDiscount = promotion != null,
                                };
                                searchProductItemResult.Add(searchProductItem);
                            }
                        }
                    }
                    #endregion

                    searchItemResult.Products.AddRange(searchProductItemResult.OrderByDescending(x => x.HasDiscount).ToList());
                    response.SearchItemResults.Add(searchItemResult);
                }
            }

            #endregion Step 2: From StoreBranches, filter Combo->ProductName match keySearch

            // Get Store/Product match keySearch && OrderBy Store has Promotion then closest distance
            response.SearchItemResults = response.SearchItemResults
                .Where(store => (store.MatchFromIndex != -1 && store.MatchToIndex > 0) || store.Products.Any(prod => prod.MatchToIndex > 0))
                .OrderByDescending(store => store.IsPromo)
                .ThenBy(store => store.Distance).ToList()
                .Take(DefaultConstants.NUMBER_STORE_ON_SEARCH)
                .ToList();

            return response;
        }

        #region private methods

        private decimal CalculateSellingPrice(decimal origin, Promotion promotion)
        {
            if (promotion.IsPercentDiscount)
            {
                var priceDiscount = origin * promotion.PercentNumber / 100;
                return (priceDiscount >= promotion.MaximumDiscountAmount && promotion.MaximumDiscountAmount > 0) ? origin - promotion.MaximumDiscountAmount : origin - priceDiscount;
            }
            return origin - promotion.MaximumDiscountAmount;
        }

        private Tuple<int, int> GetIndexKeySearchMatch(string name, string keySearch)
        {
            string cleanName = StringHelpers.RemoveSign4VietnameseString(name).ToLower();
            string cleanKeySearch = StringHelpers.RemoveSign4VietnameseString(keySearch).ToLower();

            var wordIndex = cleanName.IndexOf(cleanKeySearch);

            var matchFromIndex = wordIndex;
            var matchToIndex = matchFromIndex + keySearch.Length - 1;

            // Index >=0 mean keySearch is matched
            // Else index = -1 mean it not matched
            if (matchToIndex > 0)
            {
                return new Tuple<int, int>(matchFromIndex, matchToIndex);
            }
            return new Tuple<int, int>(0, 0);
        }



        #endregion private methods
    }
}
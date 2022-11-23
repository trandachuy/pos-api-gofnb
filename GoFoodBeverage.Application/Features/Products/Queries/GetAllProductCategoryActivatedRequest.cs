using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Combo;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Models.Promotion;
using GoogleServices.Distance;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductCategoryActivatedRequest : IRequest<GetAllProductCategoryActivatedResponse>
    {
        public Guid StoreId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid BranchId { get; set; }

        public DateTime? CurrentDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class GetAllProductCategoryActivatedResponse
    {
        public StoreDetailModel Store { get; set; }

        public IEnumerable<ComboActivatedModel> Combos { get; set; }

        public IEnumerable<ProductCategoryActivatedModel> ProductCategories { get; set; }

        public IEnumerable<GetPromotionsInBranchModel> Promotions { get; set; }
    }

    public class GetActiveAreasByBranchIdRequestHandler : IRequestHandler<GetAllProductCategoryActivatedRequest, GetAllProductCategoryActivatedResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDistanceService _googleDistanceService;

        public GetActiveAreasByBranchIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IGoogleDistanceService googleDistanceService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<GetAllProductCategoryActivatedResponse> Handle(GetAllProductCategoryActivatedRequest request, CancellationToken cancellationToken)
        {
            var store = await _unitOfWork.Stores
                .Find(s => s.Id == request.StoreId)
                .Select(s => new { s.Id, s.InitialStoreAccountId, s.Title, s.Code, s.Logo, s.Currency.Symbol, s.Address, s.Address.Country })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var thumbnail = await _unitOfWork.Staffs.GetAllStaffByInitialStoreAccount(store.InitialStoreAccountId)
                .Select(s => s.Thumbnail)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var branch = await _unitOfWork.StoreBranches.Find(c => c.Id == request.BranchId)
                .Include(item => item.Address).ThenInclude(item => item.Ward)
                .Include(item => item.Address).ThenInclude(item => item.District)
                .Include(item => item.Address).ThenInclude(item => item.City)
                .Include(item => item.Address).ThenInclude(item => item.State)
                .Include(item => item.Address).ThenInclude(item => item.Country)
                .Select(s => new { Name = s.Name, Lat = s.Address.Latitude, Lng = s.Address.Longitude, Address = s.Address})
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var brandAddress = $"{branch.Address?.Address1}, {branch.Address?.Ward?.Prefix} {branch.Address?.Ward?.Name}, {branch.Address?.District?.Prefix} {branch.Address?.District?.Name}, {branch.Address?.City?.Name}, {branch.Address?.Country?.Name}.";

            var isFavoriteStore = false;
            if (request.CustomerId != null)
            {
                isFavoriteStore = await _unitOfWork.FavoriteStores.GetAll().AnyAsync(c => c.StoreId == store.Id && c.AccountId == request.CustomerId);
            }

            double distanceBetweenPoints = await _googleDistanceService.GetDistanceBetweenPointsAsync(request.Latitude, request.Longitude, branch.Lat.Value, branch.Lng.Value, cancellationToken);

            var storeDetail = new StoreDetailModel()
            {
                Id = store.Id,
                Title = store.Title,
                BranchName = branch.Name,
                IsFavoriteStore = isFavoriteStore,
                CurrencySymbol = store.Symbol,
                Thumbnail = store.Logo,
                AddressInfo = brandAddress,
                Distance = distanceBetweenPoints
            };

            var productCategories = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(request.StoreId)
                .Include(p => p.ProductProductCategories.Where(pp => pp.Product.StatusId == (int)EnumStatus.Active && pp.Product.IsActive && pp.Product.ProductPlatforms.Count(pl => pl.PlatformId == EnumPlatform.GoFnBApp.ToGuid() && pl.ProductId == pp.ProductId) > 0).OrderBy(p => p.Product.Name)).ThenInclude(pc => pc.Product).ThenInclude(pp => pp.ProductPrices.OrderBy(x => x.CreatedTime))
                .Include(p => p.ProductProductCategories.Where(pp => pp.Product.StatusId == (int)EnumStatus.Active && pp.Product.IsActive && pp.Product.ProductPlatforms.Count(pl => pl.PlatformId == EnumPlatform.GoFnBApp.ToGuid() && pl.ProductId == pp.ProductId) > 0).OrderBy(p => p.Product.Name)).ThenInclude(pc => pc.Product).ThenInclude(po => po.ProductOptions).ThenInclude(o => o.Option).ThenInclude(o => o.OptionLevel)
                .Include(item => item.StoreBranchProductCategories)
                .Where(item => item.IsDisplayAllBranches || item.StoreBranchProductCategories.Any(item => item.StoreBranchId == request.BranchId))
                .AsNoTracking()
                .OrderBy(pc => pc.Name)
                .ToListAsync(cancellationToken: cancellationToken);

            var productCategoriesResponse = _mapper.Map<List<ProductCategoryActivatedModel>>(productCategories);

            var allProductToppingInStore = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(request.StoreId)
                .Select(p => new ProductToppingModel { Id = p.Id, Name = p.Name, Price = p.ProductPrices.FirstOrDefault().PriceValue, Thumbnail = p.Thumbnail })
                .ToListAsync();

            var productToppingIds = allProductToppingInStore.Select(pt => pt.Id).ToList();

            var productToppings = await _unitOfWork.ProductToppings.GetAll().Where(pt => productToppingIds.Contains(pt.ToppingId))
                       .Select(pt => new { pt.ProductId, pt.ToppingId })
                       .ToListAsync();

            foreach (var aProductCategory in productCategoriesResponse)
            {
                foreach (var aProduct in aProductCategory.ProductProductCategories)
                {
                    var toppingIdsNew = productToppings.Where(pt => pt.ProductId == aProduct.ProductId).Select(pt => pt.ToppingId).ToList();
                    var productToppingsModel = allProductToppingInStore.Where(pt => toppingIdsNew.Contains(pt.Id)).ToList();
                    aProduct.Product.ProductToppings = productToppingsModel;
                }
            }

            var allPromotionsInStore = await _unitOfWork.Promotions
                                   .GetAllPromotionInStore(request.StoreId)
                                   .Where(p => p.IsStopped == false && request.CurrentDate.HasValue && ((p.EndDate == null && p.StartDate != null) || (p.EndDate != null && p.EndDate >= request.CurrentDate)))
                                   .Include(p => p.PromotionBranches)
                                   .AsNoTracking()
                                   .OrderByDescending(p => p.CreatedTime)
                                   .Select(p => new { p.Id, p.StoreId, p.Name, p.PromotionTypeId, p.IsPercentDiscount, p.PercentNumber, p.MaximumDiscountAmount, p.StartDate, p.EndDate, p.IsSpecificBranch, p.PromotionBranches })
                                   .ToListAsync();

            var allPromotionsInBranch = new List<GetPromotionsInBranchModel>();

            allPromotionsInStore.ForEach(promotion =>
            {
                var promotionBranch = promotion.PromotionBranches.FirstOrDefault(pb => pb.BranchId == request.BranchId);
                if (!promotion.IsSpecificBranch || promotionBranch != null)
                {
                    var promotionReponse = new GetPromotionsInBranchModel()
                    {
                        Id = promotion.Id,
                        StoreId = promotion.StoreId,
                        Name = promotion.Name,
                        PromotionTypeId = promotion.PromotionTypeId,
                        IsPercentDiscount = promotion.IsPercentDiscount,
                        PercentNumber = promotion.PercentNumber,
                        MaximumDiscountAmount = promotion.MaximumDiscountAmount,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate
                    };
                    allPromotionsInBranch.Add(promotionReponse);
                }
            });

            var combos = await _unitOfWork.Combos
                .GetAllCombosInStoreInclude(request.StoreId)
                .Include(c => c.ComboProductPrices.Where(cpp => cpp.ProductPrice.Product.StatusId == (int)EnumStatus.Active && cpp.ProductPrice.Product.ProductPlatforms.Count(pl => pl.PlatformId == EnumPlatform.GoFnBApp.ToGuid() && pl.ProductId == cpp.ProductPrice.ProductId) > 0))
                    .ThenInclude(pr => pr.ProductPrice).ThenInclude(p => p.Product).ThenInclude(po => po.ProductOptions).ThenInclude(o => o.Option).ThenInclude(o => o.OptionLevel)
                .Include(c => c.ComboPricings.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cp => cp.ComboPricingProducts).ThenInclude(cpp => cpp.ProductPrice).ThenInclude(p => p.Product).ThenInclude(po => po.ProductOptions).ThenInclude(o => o.Option).ThenInclude(o => o.OptionLevel)
                    .Where(combo => combo.IsShowAllBranches || combo.ComboStoreBranches.Any(item => item.BranchId == request.BranchId))
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken: cancellationToken);

            var combosResponse = _mapper.Map<List<ComboActivatedModel>>(combos);

            foreach (var aCombo in combosResponse)
            {
                if(aCombo.ComboProductPrices.Count() > 0)
                {
                    foreach (var comboProductPrice in aCombo.ComboProductPrices)
                    {
                        var toppingIdsNew = productToppings.Where(pt => pt.ProductId == comboProductPrice.ProductPrice.ProductId).Select(pt => pt.ToppingId).ToList();
                        var productToppingsModel = allProductToppingInStore.Where(pt => toppingIdsNew.Contains(pt.Id)).ToList();
                        comboProductPrice.ProductPrice.Product.ProductToppings = productToppingsModel;
                    }
                } else if (aCombo.ComboPricings.Count() > 0)
                {
                    foreach (var comboPrice in aCombo.ComboPricings)
                    {
                        foreach (var comboPricingProduct in comboPrice.ComboPricingProducts)
                        {
                            var toppingIdsNew = productToppings.Where(pt => pt.ProductId == comboPricingProduct.ProductPrice.ProductId).Select(pt => pt.ToppingId).ToList();
                            var productToppingsModel = allProductToppingInStore.Where(pt => toppingIdsNew.Contains(pt.Id)).ToList();
                            comboPricingProduct.ProductPrice.Product.ProductToppings = productToppingsModel;
                        }
                    }
                }

            }

            var response = new GetAllProductCategoryActivatedResponse()
            {
                Combos = combosResponse,
                Store = storeDetail,
                ProductCategories = productCategoriesResponse,
                Promotions = allPromotionsInBranch.OrderByDescending(x => x.EndDate)
            };

            return response;
        }

        private static string FormatAddress(Address address, Country country)
        {
            var isDefaultCountry = country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;

            List<string> addressComponents = new();
            if (address != null && !string.IsNullOrWhiteSpace(address.Address1))
            {
                addressComponents.Add(address.Address1);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.Address2) && !isDefaultCountry)
            {
                addressComponents.Add(address.Address2);
            }
            if (address != null && address.Ward != null && !string.IsNullOrWhiteSpace(address.Ward.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.Ward.Name);
            }
            if (address != null && address.District != null && !string.IsNullOrWhiteSpace(address.District.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.District.Name);
            }
            if (address != null && address.City != null && !string.IsNullOrWhiteSpace(address.City.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.City.Name);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.CityTown) && !isDefaultCountry)
            {
                addressComponents.Add(address.CityTown);
            }
            if (address != null && address.State != null && !string.IsNullOrWhiteSpace(address.State.Name) && !isDefaultCountry)
            {
                addressComponents.Add(address.State.Name);
            }
            if (!string.IsNullOrWhiteSpace(country.Nicename))
            {
                addressComponents.Add(country.Nicename);
            }

            return string.Join(", ", addressComponents);
        }
    }
}

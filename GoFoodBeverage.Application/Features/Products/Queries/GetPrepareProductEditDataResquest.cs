using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using System;
using GoFoodBeverage.Models.Option;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Unit;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Tax;
using GoFoodBeverage.Models.Store;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetPrepareProductEditDataResquest : IRequest<GetPrepareProductEditDataResponse>
    {
        public Guid ProductId { get; set; }
    }

    public class GetPrepareProductEditDataResponse
    {
        public ProductEditResponseModel Product { get; set; }

        public IEnumerable<ProductToppingModel> ProductToppings { get; set; }
    }

    public class GetProductEditRequestHandler : IRequestHandler<GetPrepareProductEditDataResquest, GetPrepareProductEditDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetProductEditRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;

        }

        public async Task<GetPrepareProductEditDataResponse> Handle(GetPrepareProductEditDataResquest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var product = _unitOfWork.Products
                                   .GetProductByIdInStore(loggedUser.StoreId.Value, request.ProductId)
                                   .Where(p => p.IsActive == true)
                                   .Include(p => p.ProductPrices.OrderBy(x => x.CreatedTime)).ThenInclude(p => p.ProductPriceMaterials).ThenInclude(x => x.Material).ThenInclude(x => x.Unit)
                                   .Include(p => p.Unit)
                                   .Include(p => p.ProductProductCategories)
                                   .Include(p => p.Tax)
                                   .Include(p => p.ProductOptions)
                                   .Include(p => p.ProductPlatforms)
                                   .FirstOrDefault();

            var productEdit = _mapper.Map<ProductEditResponseModel>(product);
            productEdit.ProductCategoryId = product.ProductProductCategories.FirstOrDefault(x => x.ProductId == request.ProductId)?.ProductCategoryId;
            productEdit.ListOptionIds = product.ProductOptions.Select(x => x.OptionId);
            productEdit.ListPlatformIds = product.ProductPlatforms.Select(x => x.PlatformId);

            var listProductInventoryData = new List<ProductInventoryData>();
            foreach (var productPrice in product.ProductPrices)
            {
                var productInventoryData = new ProductInventoryData
                {
                    PriceName = productPrice.PriceName
                };

                var productPriceMaterials = product.ProductPrices.FirstOrDefault(x => x.PriceName == productPrice.PriceName);
                productInventoryData.ListProductPriceMaterial = productPriceMaterials.ProductPriceMaterials.Select(ppm => new ProductInventoryTableData
                {
                    Key = ppm.MaterialId,
                    Unit = ppm.Material?.Unit?.Name,
                    Quantity = ppm.Quantity,
                    UnitCost = ppm.Material?.CostPerUnit != null ? ppm.Material.CostPerUnit.Value : 0,
                    Material = ppm.Material?.Name,
                    Cost = (ppm.Quantity * (ppm.Material?.CostPerUnit != null ? ppm.Material.CostPerUnit.Value : 0))
                });

                productInventoryData.TotalCost = productInventoryData.ListProductPriceMaterial.Sum(x => (x.Quantity * x.UnitCost));
                listProductInventoryData.Add(productInventoryData);
            }

            var options = await _unitOfWork.Options
                .GetAllOptionsInStore(loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<OptionModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            productEdit.Options = options.AsEnumerable();

            var platforms = await _unitOfWork.Platforms
                .GetActivePlatforms()
                .AsNoTracking()
                .ProjectTo<PlatformModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            productEdit.Platforms = platforms.AsEnumerable();

            var units = await _unitOfWork.Units
                .GetAllUnitsInStore(loggedUser.StoreId)
                .OrderByDescending(u => u.Position)
                .AsNoTracking()
                .ProjectTo<UnitModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            productEdit.Units = units;

            var taxes = await _unitOfWork.Taxes
                .GetAllTaxInStore(loggedUser.StoreId.Value)
                .Where(t => t.TaxTypeId == (int)EnumTaxType.SellingTax)
                .AsNoTracking()
                .ProjectTo<TaxTypeModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            productEdit.Taxes = taxes;

            var materials = await _unitOfWork.Materials
               .GetAllMaterialsInStore(loggedUser.StoreId)
               .AsNoTracking()
               .ProjectTo<MaterialModel>(_mapperConfiguration)
               .ToListAsync(cancellationToken: cancellationToken);
            productEdit.Materials = materials.AsEnumerable();

            productEdit.ProductInventoryData = listProductInventoryData.AsEnumerable();
            var allProductCategoriesInStore = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                .Include(pc => pc.ProductProductCategories)
                .ThenInclude(ppc => ppc.Product)
                .OrderBy(pc => pc.Priority)
                .ToListAsync(cancellationToken: cancellationToken);
            var productCategoryListResponse = _mapper.Map<List<ProductCategoryModel>>(allProductCategoriesInStore);
            productEdit.AllProductCategories = productCategoryListResponse;
            if (productEdit.ProductCategoryId != null)
            {
                productEdit.ProductCategoryName = productCategoryListResponse.FirstOrDefault(x => x.Id == productEdit.ProductCategoryId.Value)?.Name;
            }

            var productToppings = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(loggedUser.StoreId.Value)
                .ProjectTo<ProductToppingModel>(_mapperConfiguration)
                .ToListAsync();

            if (!productEdit.IsTopping)
            {
                productEdit.ProductToppingIds = await _unitOfWork.ProductToppings
                .GetAll()
                .Where(pt => pt.StoreId == loggedUser.StoreId && pt.ProductId == request.ProductId)
                .Select(x => x.ToppingId)
                .ToListAsync();
            }

            var response = new GetPrepareProductEditDataResponse()
            {
                Product = productEdit,
                ProductToppings = productToppings
            };

            return response;
        }
    }
}

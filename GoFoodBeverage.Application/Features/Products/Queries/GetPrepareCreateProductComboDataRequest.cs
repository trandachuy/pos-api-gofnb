using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Unit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetPrepareCreateProductComboDataRequest : IRequest<GetPrepareCreateProductComboDataResponse>
    {
    }

    public class GetPrepareCreateProductComboDataResponse
    {
        public IEnumerable<StoreBranchModel> Branches { get; set; }

        public IEnumerable<ProductModel> Products { get; set; }
        public IEnumerable<ProductModel> ProductsWithCategory { get; set; }

        public IEnumerable<ProductCategoryModel> ProductCategories { get; set; }
    }

    public class GetPrepareCreateProductComboDataRequestHandler : IRequestHandler<GetPrepareCreateProductComboDataRequest, GetPrepareCreateProductComboDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetPrepareCreateProductComboDataRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetPrepareCreateProductComboDataResponse> Handle(GetPrepareCreateProductComboDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            ///Get list branches
            var branches = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(loggedUser.StoreId.Value)
                .ProjectTo<StoreBranchModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            ///Get list product categories
            var productCategories = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                .ProjectTo<ProductCategoryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            ///Get product in product category platform POS & App
            var productsWithCategory = await _unitOfWork.ProductProductCategories
                .GetAllProductInStoreActive(loggedUser.StoreId.Value)
                .ProjectTo<ProductModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            var productsWithCategoryIds = productsWithCategory.Select(x => x.Id).ToList();

            var productsWithCategoryInPlatform = _unitOfWork.ProductPlatforms.Find(p => p.StoreId == loggedUser.StoreId && productsWithCategoryIds.Any(pid => pid == p.ProductId)).ToList();

            var productsWithCategoryInPlatformPos = productsWithCategoryInPlatform.Where(x => x.PlatformId == EnumPlatform.POS.ToGuid()).Select(x => x.ProductId);
            var productsWithCategoryInPlatformApp = productsWithCategoryInPlatform.Where(x => x.PlatformId == EnumPlatform.GoFnBApp.ToGuid()).Select(x => x.ProductId);
            var productsWithCategoryInPlatformPosAndAppIds = productsWithCategoryInPlatformPos.Intersect(productsWithCategoryInPlatformApp);

            var productsWithCategoryModel = productsWithCategory.Where(p => productsWithCategoryInPlatformPosAndAppIds.Any(ppid => ppid == p.Id)).ToList();

            ///Get product list in store platform POS & App
            var products = await _unitOfWork.Products
                .GetAllProductInStore(loggedUser.StoreId.Value)
                .Where(x => x.StatusId == (int)EnumStatus.Active && x.IsActive == true && !x.IsTopping)
                .Include(p => p.ProductPrices)
                .Include(p => p.Unit)
                .ProjectTo<ProductModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            var productIds = products.Select(x => x.Id).ToList();

            var productInPlatform = _unitOfWork.ProductPlatforms.Find(p => p.StoreId == loggedUser.StoreId && productIds.Any(pid => pid == p.ProductId)).ToList();

            var productInPlatformPos = productInPlatform.Where(x => x.PlatformId == EnumPlatform.POS.ToGuid()).Select(x => x.ProductId);
            var productInPlatformApp = productInPlatform.Where(x => x.PlatformId == EnumPlatform.GoFnBApp.ToGuid()).Select(x => x.ProductId);
            var productInPlatformPosAndAppIds = productInPlatformPos.Intersect(productInPlatformApp);

            var productsModel = products.Where(p => productInPlatformPosAndAppIds.Any(ppid => ppid == p.Id)).ToList();

            var response = new GetPrepareCreateProductComboDataResponse()
            {
                Branches = branches,
                Products = productsModel,
                ProductsWithCategory = productsWithCategoryModel,
                ProductCategories = productCategories
            };

            return response;
        }
    }
}

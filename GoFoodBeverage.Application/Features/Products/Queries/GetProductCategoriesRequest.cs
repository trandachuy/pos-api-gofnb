using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Product;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetProductCategoriesRequest : IRequest<GetProductCategoriesResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetProductCategoriesResponse
    {
        public IEnumerable<ProductCategoryModel> ProductCategories { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetProductCategoriesRequestHandler : IRequestHandler<GetProductCategoriesRequest, GetProductCategoriesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductCategoriesRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetProductCategoriesResponse> Handle(GetProductCategoriesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var allProductCategoriesInStore = new PagingExtensions.Pager<ProductCategory>(new List<ProductCategory>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                allProductCategoriesInStore = await _unitOfWork.ProductCategories
                    .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                    .Include(pc => pc.ProductProductCategories)
                    .ThenInclude(ppc => ppc.Product)
                    .OrderBy(pc => pc.Priority)
                    .ThenBy(x => x.Name)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                allProductCategoriesInStore = await _unitOfWork.ProductCategories
                   .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                   .Where(pc => pc.Name.ToLower().Contains(keySearch))
                   .Include(pc => pc.ProductProductCategories)
                   .ThenInclude(ppc => ppc.Product)
                   .OrderBy(pc => pc.Priority)
                   .ThenBy(x => x.Name)
                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listAllProductCategoryInStore = allProductCategoriesInStore.Result;
            var productCategoryListResponse = _mapper.Map<List<ProductCategoryModel>>(listAllProductCategoryInStore);
            productCategoryListResponse.ForEach(pc =>
            {
                var productCategory = listAllProductCategoryInStore.FirstOrDefault(i => i.Id == pc.Id);
                var products = productCategory.ProductProductCategories.Select(p => p.Product);
                pc.Products = _mapper.Map<IEnumerable<ProductDatatableModel>>(products);
                pc.NumberOfProduct = pc.Products.Count();
                pc.No = productCategoryListResponse.IndexOf(pc) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetProductCategoriesResponse()
            {
                PageNumber = request.PageNumber,
                Total = allProductCategoriesInStore.Total,
                ProductCategories = productCategoryListResponse
            };

            return response;
        }
    }
}

using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Models.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetProductsByFilterRequest : IRequest<GetProductsByFilterResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public int? StatusId { get; set; }

        public Guid? PlatformId { get; set; }
    }

    public class GetProductsByFilterResponse
    {
        public IEnumerable<ProductDatatableModel> Products { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetProductsByFilterRequestHandler : IRequestHandler<GetProductsByFilterRequest, GetProductsByFilterResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsByFilterRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetProductsByFilterResponse> Handle(GetProductsByFilterRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var products = _unitOfWork.Products
                .GetAllProductInStore(loggedUser.StoreId.Value)
                .Where(p => p.IsActive == true);

            if (products != null)
            {
                if (request.ProductCategoryId != null)
                {
                    /// Find Products by Product categoryId
                    var productIdsInProductCategory = _unitOfWork.ProductProductCategories
                        .Find(m => m.StoreId == loggedUser.StoreId && m.ProductCategoryId == request.ProductCategoryId)
                        .Select(m => m.ProductId);

                    products = products.Where(x => productIdsInProductCategory.Contains(x.Id));
                }

                if (request.StatusId != null)
                {
                    products = products.Where(x => x.StatusId == request.StatusId);
                }

                if (request.PlatformId != null)
                {
                    var productIds = _unitOfWork.ProductPlatforms.GetAll().Where(pp => pp.StoreId == loggedUser.StoreId && pp.PlatformId == request.PlatformId);
                    products = products.Where(p => productIds.Any(x => x.ProductId == p.Id));
                }

                if (!string.IsNullOrEmpty(request.KeySearch))
                {
                    string keySearch = request.KeySearch.Trim().ToLower();
                    products = products.Where(g => g.Name.ToLower().Contains(keySearch));
                }
            }

            var allProductsInStore = await products
                                    .AsNoTracking()
                                    .Include(p => p.ProductPrices)
                                    .Include(p => p.ProductChannels)
                                    .ThenInclude(pc => pc.Channel)
                                    .OrderByDescending(p => p.CreatedTime)
                                    .ToPaginationAsync(request.PageNumber, request.PageSize);
            var pagingResult = allProductsInStore.Result;
            var productListResponse = _mapper.Map<List<ProductDatatableModel>>(pagingResult);
            productListResponse.ForEach(p =>
            {
                var product = pagingResult.FirstOrDefault(i => i.Id == p.Id);
                var channels = product.ProductChannels.Select(p => p.Channel);
                p.Channels = _mapper.Map<IEnumerable<ChannelModel>>(channels);
                p.No = productListResponse.IndexOf(p) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetProductsByFilterResponse()
            {
                PageNumber = request.PageNumber,
                Total = allProductsInStore.Total,
                Products = productListResponse
            };

            return response;
        }
    }
}

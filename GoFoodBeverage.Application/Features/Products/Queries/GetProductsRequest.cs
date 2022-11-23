using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Extensions;
using MoreLinq;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetProductsRequest : IRequest<GetProductsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetProductsResponse
    {
        public IEnumerable<ProductDatatableModel> Products { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetProductsRequestHandler : IRequestHandler<GetProductsRequest, GetProductsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetProductsResponse> Handle(GetProductsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var allProductInStore = new PagingExtensions.Pager<Product>(new List<Product>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                allProductInStore = await _unitOfWork.Products
                                   .GetAllProductInStore(loggedUser.StoreId.Value)
                                   .Where(p => p.IsActive == true)
                                   .AsNoTracking()
                                   .Include(p => p.ProductPrices.OrderBy(x => x.CreatedTime))
                                   .Include(p => p.ProductChannels).ThenInclude(pc => pc.Channel)
                                   .Include(p => p.ProductPlatforms).ThenInclude(pc => pc.Platform)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                allProductInStore = await _unitOfWork.Products
                                   .GetAllProductInStore(loggedUser.StoreId.Value)
                                   .Where(p => p.IsActive == true)
                                   .AsNoTracking()
                                   .Where(s => s.Name.ToLower().Contains(keySearch))
                                   .Include(p => p.ProductPrices.OrderBy(x => x.CreatedTime))
                                   .Include(p => p.ProductChannels).ThenInclude(pc => pc.Channel)
                                   .Include(p => p.ProductPlatforms).ThenInclude(pc => pc.Platform)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listAllProductInStore = allProductInStore.Result;
            var productListResponse = _mapper.Map<List<ProductDatatableModel>>(listAllProductInStore);

            var channels = listAllProductInStore.SelectMany(p => p.ProductChannels.Select(pc => pc.Channel)).DistinctBy(c => c.Id);
            var channelModels = _mapper.Map<IEnumerable<ChannelModel>>(channels);

            var platforms = listAllProductInStore.SelectMany(p => p.ProductPlatforms.Select(pc => pc.Platform)).DistinctBy(c => c.Id);
            var platformModels = _mapper.Map<IEnumerable<PlatformModel>>(platforms);

            productListResponse.ForEach(p =>
            {
                var product = listAllProductInStore.FirstOrDefault(i => i.Id == p.Id);

                var productChannels = product.ProductChannels.Select(p => p.Channel);
                p.Channels = channelModels.Where(cm => productChannels.Any(c => c.Id == cm.Id));

                var productPlatforms = product.ProductPlatforms.Select(p => p.Platform);
                p.Platforms = platformModels.Where(pm => productPlatforms.Any(c => c.Id == pm.Id));

                p.No = productListResponse.IndexOf(p) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetProductsResponse()
            {
                PageNumber = request.PageNumber,
                Total = allProductInStore.Total,
                Products = productListResponse
            };

            return response;
        }
    }
}

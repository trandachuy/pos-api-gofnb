using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Models.Product;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductsRequest : IRequest<GetAllProductsResponse>
    {

    }

    public class GetAllProductsResponse
    {
        public IEnumerable<ProductModel> Products { get; set; }
    }

    public class GetAllProductsRequestHandler : IRequestHandler<GetAllProductsRequest, GetAllProductsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllProductsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllProductsResponse> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var products = await _unitOfWork.Products
                .GetAllProductInStore(loggedUser.StoreId.Value)
                .Where(p => p.IsActive == true)
                .AsNoTracking()
                .ProjectTo<ProductModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllProductsResponse()
            {
                Products = products
            };

            return response;
        }
    }
}

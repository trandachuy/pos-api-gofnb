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

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductsWithCategoryRequest : IRequest<GetAllProductsWithCategoryResponse>
    {

    }

    public class GetAllProductsWithCategoryResponse
    {
        public IEnumerable<ProductModel> Products { get; set; }
    }

    public class GetAllProductsWithCategoryRequestHandler : IRequestHandler<GetAllProductsWithCategoryRequest, GetAllProductsWithCategoryResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllProductsWithCategoryRequestHandler(
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

        public async Task<GetAllProductsWithCategoryResponse> Handle(GetAllProductsWithCategoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var products = await _unitOfWork.ProductProductCategories
                   .GetAllProductInStoreActive(loggedUser.StoreId.Value)
                   .ProjectTo<ProductModel>(_mapperConfiguration)
                   .ToListAsync(cancellationToken);

            var response = new GetAllProductsWithCategoryResponse()
            {
                Products = products
            };

            return response;
        }
    }
}

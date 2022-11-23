using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Product;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductToppingsRequest : IRequest<GetAllProductToppingsResponse>
    {

    }

    public class GetAllProductToppingsResponse
    {
        public IEnumerable<ProductToppingModel> ProductToppings { get; set; }
    }

    public class GetAllProductToppingsRequestHandler : IRequestHandler<GetAllProductToppingsRequest, GetAllProductToppingsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllProductToppingsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllProductToppingsResponse> Handle(GetAllProductToppingsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var productTopping = await _unitOfWork.Products
                .GetAllToppingActivatedInStore(loggedUser.StoreId.Value)
                .ProjectTo<ProductToppingModel>(_mapperConfiguration)
                .ToListAsync();

            var response = new GetAllProductToppingsResponse()
            {
                ProductToppings = productTopping
            };

            return response;
        }
    }
}

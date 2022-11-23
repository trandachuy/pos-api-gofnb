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
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductsActiveRequest : IRequest<GetAllProductsActiveResponse>
    {

    }

    public class GetAllProductsActiveResponse
    {
        public IEnumerable<ProductModel> Products { get; set; }
    }

    public class GetAllProductsActiveRequestHandler : IRequestHandler<GetAllProductsActiveRequest, GetAllProductsActiveResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllProductsActiveRequestHandler(
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

        public async Task<GetAllProductsActiveResponse> Handle(GetAllProductsActiveRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var products = await _unitOfWork.Products
                 .GetAllProductInStore(loggedUser.StoreId.Value)
                 .Where(x => x.StatusId == (int)EnumStatus.Active && !x.IsTopping && x.IsActive == true)
                 .Include(p => p.ProductPrices)
                 .Include(p => p.Unit)
                 .ProjectTo<ProductModel>(_mapperConfiguration)
                 .ToListAsync(cancellationToken);

            var response = new GetAllProductsActiveResponse()
            {
                Products = products
            };

            return response;
        }
    }
}

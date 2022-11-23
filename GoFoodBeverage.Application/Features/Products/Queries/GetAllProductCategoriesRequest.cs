using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Product;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetAllProductCategoriesRequest : IRequest<GetAllProductCategoriesResponse>
    {
    }

    public class GetAllProductCategoriesResponse
    {
        public IEnumerable<ProductCategoryModel> AllProductCategories { get; set; }
    }

    public class GetAllProductCategoriesRequestHandler : IRequestHandler<GetAllProductCategoriesRequest, GetAllProductCategoriesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllProductCategoriesRequestHandler(
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

        public async Task<GetAllProductCategoriesResponse> Handle(GetAllProductCategoriesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var allProductCategoriesInStore = await _unitOfWork.ProductCategories
                    .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                    .AsNoTracking()
                    .Include(pc => pc.ProductProductCategories)
                    .ThenInclude(ppc => ppc.Product)
                    .ProjectTo<ProductCategoryModel>(_mapperConfiguration)
                    .OrderBy(pc => pc.Name)
                    .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllProductCategoriesResponse()
            {
                AllProductCategories = allProductCategoriesInStore
            };

            return response;
        }
    }
}

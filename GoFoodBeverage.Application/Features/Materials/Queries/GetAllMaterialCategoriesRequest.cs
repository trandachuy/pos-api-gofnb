using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetAllMaterialCategoriesRequest : IRequest<GetAllMaterialCategoriesResponse>
    {

    }

    public class GetAllMaterialCategoriesResponse
    {
        public IEnumerable<MaterialCategoryModel> MaterialCategories { get; set; }
    }

    public class GetAllMaterialCategoriesRequestHandler : IRequestHandler<GetAllMaterialCategoriesRequest, GetAllMaterialCategoriesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllMaterialCategoriesRequestHandler(
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

        public async Task<GetAllMaterialCategoriesResponse> Handle(GetAllMaterialCategoriesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialCategories = await _unitOfWork.MaterialCategories
                .GetAllMaterialCategoriesInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .ProjectTo<MaterialCategoryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            var response = new GetAllMaterialCategoriesResponse()
            {
                MaterialCategories = materialCategories
            };

            return response;
        }
    }
}

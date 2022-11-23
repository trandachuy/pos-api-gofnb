using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Material;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetAllMaterialFilterRequest : IRequest<GetAllMaterialsFilterResponse>
    {

    }

    public class GetAllMaterialsFilterResponse
    {
        public IEnumerable<MaterialModel> Materials { get; set; }
    }

    public class GetAllMaterialFilterRequestHandler : IRequestHandler<GetAllMaterialFilterRequest, GetAllMaterialsFilterResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllMaterialFilterRequestHandler(
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

        public async Task<GetAllMaterialsFilterResponse> Handle(GetAllMaterialFilterRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materials = await _unitOfWork.Materials
                .Find(x => x.StoreId == loggedUser.StoreId.Value)
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ProjectTo<MaterialModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllMaterialsFilterResponse()
            {
                Materials = materials
            };

            return response;
        }

    }
}

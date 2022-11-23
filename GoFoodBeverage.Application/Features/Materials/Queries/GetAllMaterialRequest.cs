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
using System.Linq;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetAllMaterialRequest : IRequest<GetAllMaterialsResponse>
    {
        
    }

    public class GetAllMaterialsResponse
    {
        public IEnumerable<MaterialModel> Materials { get; set; }
    }

    public class GetAllMaterialRequestHandler : IRequestHandler<GetAllMaterialRequest, GetAllMaterialsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllMaterialRequestHandler(
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

        public async Task<GetAllMaterialsResponse> Handle(GetAllMaterialRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materials = await _unitOfWork.Materials
                .GetAllMaterialsActivatedInStore(loggedUser.StoreId)
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ProjectTo<MaterialModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllMaterialsResponse()
            {
                Materials = materials
            };

            return response;
        }

    }
}

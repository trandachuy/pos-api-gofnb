using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.POS.Models.Material;

namespace GoFoodBeverage.POS.Application.Features.Materials.Queries
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllMaterialRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllMaterialsResponse> Handle(GetAllMaterialRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materials = await _unitOfWork.Materials
                .GetAllMaterialsInStore(loggedUser.StoreId)
                .AsNoTracking()
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

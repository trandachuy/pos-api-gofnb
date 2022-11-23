using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetAllActivePlatformRequest : IRequest<GetAllActivePlatformResponse>
    {

    }

    public class GetAllActivePlatformResponse
    {
        public IEnumerable<PlatformModel> Platforms { get; set; }
    }

    public class GetAllActivePlatformRequestHandler : IRequestHandler<GetAllActivePlatformRequest, GetAllActivePlatformResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllActivePlatformRequestHandler(
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

        public async Task<GetAllActivePlatformResponse> Handle(GetAllActivePlatformRequest request, CancellationToken cancellationToken)
        {
            var platforms = await _unitOfWork.Platforms
                .GetActivePlatforms()
                .AsNoTracking()
                .ProjectTo<PlatformModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllActivePlatformResponse()
            {
                Platforms = platforms
            };

            return response;
        }
    }
}

using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Unit;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Units.Queries
{
    public class GetUnitsRequest : IRequest<GetUnitsResponse>
    {
        
    }

    public class GetUnitsResponse
    {
        public IEnumerable<UnitModel> Units { get; set; }
    }

    public class GetUnitsRequestHandler : IRequestHandler<GetUnitsRequest, GetUnitsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetUnitsRequestHandler(
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

        public async Task<GetUnitsResponse> Handle(GetUnitsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var units = await _unitOfWork.Units
                .GetAllUnitsInStore(loggedUser.StoreId)
                .OrderByDescending(u => u.Position)
                .AsNoTracking()
                .ProjectTo<UnitModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetUnitsResponse()
            {
                Units = units
            };

            return response;
        }
    }
}

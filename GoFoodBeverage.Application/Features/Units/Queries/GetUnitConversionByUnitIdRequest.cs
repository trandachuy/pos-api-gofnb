using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Unit;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using System.Linq;
using System;

namespace GoFoodBeverage.Application.Features.Units.Queries
{
    public class GetUnitConversionByUnitIdRequest : IRequest<GetUnitConversionByUnitIdResponse>
    {
        public Guid? UnitId { get; set; }
    }

    public class GetUnitConversionByUnitIdResponse
    {
        public UnitConversionDto UnitConversion { get; set; }
    }

    public class GetUnitConversionByUnitIdRequestHandler : IRequestHandler<GetUnitConversionByUnitIdRequest, GetUnitConversionByUnitIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetUnitConversionByUnitIdRequestHandler(
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

        public async Task<GetUnitConversionByUnitIdResponse> Handle(GetUnitConversionByUnitIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var unitConversion = await _unitOfWork.UnitConversions
                .GetAllUnitConversionsInStore(loggedUser.StoreId)
                .Where(u => u.UnitId == request.UnitId)
                .ProjectTo<UnitConversionDto>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var response = new GetUnitConversionByUnitIdResponse()
            {
                UnitConversion = unitConversion
            };

            return response;
        }
    }
}

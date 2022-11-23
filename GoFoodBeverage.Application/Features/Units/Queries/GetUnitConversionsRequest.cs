using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Unit;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Units.Queries
{
    public class GetUnitConversionsRequest : IRequest<GetUnitConversionsResponse>
    {
        
    }

    public class GetUnitConversionsResponse
    {
        public IEnumerable<UnitConversionUnitDto> UnitConversions { get; set; }
    }

    public class GetUnitConversionsRequestHandler : IRequestHandler<GetUnitConversionsRequest, GetUnitConversionsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetUnitConversionsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetUnitConversionsResponse> Handle(GetUnitConversionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var unitConversions = await _unitOfWork.UnitConversions
                .GetAllUnitConversionsInStore(loggedUser.StoreId)
                .Include(u => u.Unit)
                .AsNoTracking()
                .ProjectTo<UnitConversionUnitDto>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetUnitConversionsResponse()
            {
                UnitConversions = unitConversions
            };

            return response;
        }
    }
}

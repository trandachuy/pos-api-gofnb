using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Unit;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Units.Queries
{
    public class GetUnitConversionsByMaterialIdRequest : IRequest<GetUnitConversionsByMaterialIdResponse>
    {
        public Guid MaterialId { get; set; }
    }

    public class GetUnitConversionsByMaterialIdResponse
    {
        public IEnumerable<UnitConversionUnitDto> UnitConversions { get; set; }
    }

    public class GetUnitConversionsByMaterialIdRequestHandler : IRequestHandler<GetUnitConversionsByMaterialIdRequest, GetUnitConversionsByMaterialIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetUnitConversionsByMaterialIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetUnitConversionsByMaterialIdResponse> Handle(GetUnitConversionsByMaterialIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var unitConversions = await _unitOfWork.UnitConversions
                .GetUnitConversionsByMaterialIdInStore(request.MaterialId, loggedUser.StoreId)
                .AsNoTracking()
                .Include(u => u.Unit)
                .ProjectTo<UnitConversionUnitDto>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetUnitConversionsByMaterialIdResponse()
            {
                UnitConversions = unitConversions
            };

            return response;
        }
    }
}

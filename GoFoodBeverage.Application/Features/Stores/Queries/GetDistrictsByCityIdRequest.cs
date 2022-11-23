using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Address;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetDistrictsByCityIdRequest : IRequest<GetDistrictsByCityIdResponse>
    {
        public Guid? CityId { get; set; }
    }

    public class GetDistrictsByCityIdResponse
    {
        public IList<DistrictModel> Districts { get; set; }
    }

    public class GetDistrictsByCityIdRequestHandler : IRequestHandler<GetDistrictsByCityIdRequest, GetDistrictsByCityIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetDistrictsByCityIdRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDistrictsByCityIdResponse> Handle(GetDistrictsByCityIdRequest request, CancellationToken cancellationToken)
        {
            ThrowError.BadRequestAgainstNull(request.CityId, "Please enter the CityId");

            var districts = await _unitOfWork.Districts
                .GetDistrictsByCityId(request.CityId.Value)
                .ProjectTo<DistrictModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetDistrictsByCityIdResponse()
            {
                Districts = districts
            };

            return response;
        }
    }
}

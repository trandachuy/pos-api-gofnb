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
using System;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetCitiesByCountryIdRequest : IRequest<GetCitiesByCountryIdResponse>
    {
        public Guid? CountryId { get; set; }
    }

    public class GetCitiesByCountryIdResponse
    {
        public IList<CityModel> Cities { get; set; }
    }

    public class GetCitiesByCountryIdRequestHandler : IRequestHandler<GetCitiesByCountryIdRequest, GetCitiesByCountryIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetCitiesByCountryIdRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetCitiesByCountryIdResponse> Handle(GetCitiesByCountryIdRequest request, CancellationToken cancellationToken)
        {
            ThrowError.BadRequestAgainstNull(request.CountryId, "Please enter the CountryId");

            var cities = await _unitOfWork.Cities
                .GetCitiesByCountryId(request.CountryId.Value)
                .ProjectTo<CityModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetCitiesByCountryIdResponse()
            {
                Cities = cities
            };

            return response;
        }
    }
}

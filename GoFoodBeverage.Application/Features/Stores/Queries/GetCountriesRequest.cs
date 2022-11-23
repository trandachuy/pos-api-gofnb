using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Address;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;


namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetCountriesRequest : IRequest<GetCountriesResponse>
    {

    }

    public class GetCountriesResponse
    {

        public List<CountryModel> Countries { get; set; }

    }

    public class GetCountriesRequestHandler : IRequestHandler<GetCountriesRequest, GetCountriesResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly MapperConfiguration _mapperConfiguration;

        public GetCountriesRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        /// <summary>
        /// This method is used to handle the current request to get all countries from the database.
        /// </summary>
        /// <param name="request">The HTTP request data.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<GetCountriesResponse> Handle(GetCountriesRequest request, CancellationToken cancellationToken)
        {

            // Get all countries from the database.
            var countries = await _unitOfWork.Countries
                .GetAll()
                .AsNoTracking()
                .ProjectTo<CountryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

             // Create a new object to return to the action method.
            var response = new GetCountriesResponse()
            {
                Countries = countries
            };

            // Return data.
            return response;

        }
    }
}

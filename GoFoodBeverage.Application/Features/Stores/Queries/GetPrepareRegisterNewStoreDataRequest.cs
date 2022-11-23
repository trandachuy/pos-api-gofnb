using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Address;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetPrepareRegisterNewStoreDataRequest : IRequest<GetPrepareRegisterNewStoreDataResponse>
    {
        
    }

    public class GetPrepareRegisterNewStoreDataResponse
    {
        public CountryModel DefaultCountry { get; set; }

        public CountryModel DefaultCountryStore { get; set; }

        public IEnumerable<BusinessAreaModel> BusinessAreas { get; set; }

        public IEnumerable<CurrencyModel> Currencies { get; set; }

        public IEnumerable<CountryModel> Countries { get; set; }

        public IEnumerable<StateModel> States { get; set; }

        public IEnumerable<CityModel> Cities { get; set; }

        public IEnumerable<DistrictModel> Districts { get; set; }

        public IEnumerable<WardModel> Wards { get; set; }
    }

    public class GetPrepareRegisterNewStoreDataRequestHandler : IRequestHandler<GetPrepareRegisterNewStoreDataRequest, GetPrepareRegisterNewStoreDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetPrepareRegisterNewStoreDataRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetPrepareRegisterNewStoreDataResponse> Handle(GetPrepareRegisterNewStoreDataRequest request, CancellationToken cancellationToken)
        {
            var businessAreas = await _unitOfWork.BusinessAreas
                .GetAll()
                .AsNoTracking()
                .ProjectTo<BusinessAreaModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var currencies = await _unitOfWork.Currencies
                .GetAll()
                .AsNoTracking()
                .ProjectTo<CurrencyModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var countries = await _unitOfWork.Countries
                .GetAll()
                .AsNoTracking()
                .ProjectTo<CountryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var states = await _unitOfWork.States.GetAll()
                .AsNoTracking()
                .ProjectTo<StateModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var defaultCountry = countries.FirstOrDefault(c => c.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO);

            var cities = await _unitOfWork.Cities.GetAll()
                .AsNoTracking()
                .ProjectTo<CityModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var districts = await _unitOfWork.Districts.GetAll()
                .AsNoTracking()
                .ProjectTo<DistrictModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var wards = await _unitOfWork.Wards.GetAll()
                .AsNoTracking()
                .ProjectTo<WardModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetPrepareRegisterNewStoreDataResponse()
            {
                DefaultCountry = defaultCountry,
                Countries = countries,
                States = states,
                BusinessAreas = businessAreas,
                Currencies = currencies,
                Cities = cities,
                Districts = districts,
                Wards = wards
            };

            return response;
        }
    }
}

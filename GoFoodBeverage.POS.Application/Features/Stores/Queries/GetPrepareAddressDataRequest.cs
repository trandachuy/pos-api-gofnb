using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.POS.Models.Address;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.POS.Application.Features.Stores.Queries
{
    public class GetPrepareAddressDataRequest : IRequest<GetPrepareAddressDataDataResponse>
    {

    }

    public class GetPrepareAddressDataDataResponse
    {
        public CountryModel DefaultCountry { get; set; }

        public CountryModel DefaultCountryStore { get; set; }

        public IEnumerable<CountryModel> Countries { get; set; }

        public IEnumerable<StateModel> States { get; set; }

        public IEnumerable<CityModel> Cities { get; set; }

        public IEnumerable<DistrictModel> Districts { get; set; }

        public IEnumerable<WardModel> Wards { get; set; }
    }

    public class GetPrepareAddressDataRequestHandler : IRequestHandler<GetPrepareAddressDataRequest, GetPrepareAddressDataDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetPrepareAddressDataRequestHandler(
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

        public async Task<GetPrepareAddressDataDataResponse> Handle(GetPrepareAddressDataRequest request, CancellationToken cancellationToken)
        {

            var countries = await _unitOfWork.Countries
                .GetAll()
                .AsNoTracking()
                .ProjectTo<CountryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var states = await _unitOfWork.States.GetAll()
                .AsNoTracking()
                .ProjectTo<StateModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

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

            var defaultCountry = countries.FirstOrDefault(c => c.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO);
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            var defaultCountryStore = await _unitOfWork.Countries.GetCountryByStoreIdAsync(loggerUser.StoreId.Value);
            var defaultCountryStoreModel = _mapper.Map<CountryModel>(defaultCountryStore);

            var response = new GetPrepareAddressDataDataResponse()
            {
                Countries = countries,
                States = states,   
                Cities = cities,
                Districts = districts,
                Wards = wards,
                DefaultCountry = defaultCountry,
                DefaultCountryStore = defaultCountryStoreModel
            };

            return response;
        }
    }
}

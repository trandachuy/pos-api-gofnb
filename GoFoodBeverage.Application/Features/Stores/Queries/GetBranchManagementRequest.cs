using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Address;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetBranchManagementRequest : IRequest<GetBranchManagementsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetBranchManagementsResponse
    {
        public IEnumerable<StoreBranchModel> BranchManagements { get; set; }

        public int Total { get; set; }
    }

    public class GetBranchManagementRequestHandler : IRequestHandler<GetBranchManagementRequest, GetBranchManagementsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetBranchManagementRequestHandler(
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

        public async Task<GetBranchManagementsResponse> Handle(GetBranchManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync();

            var branchManagements = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(loggedUser.StoreId)
                .AsNoTracking()
                .Include(b => b.Address).ThenInclude(ad => ad.City)
                .Include(b => b.Address).ThenInclude(ad => ad.District)
                .Include(b => b.Address).ThenInclude(ad => ad.Ward)
                .Include(b => b.Address).ThenInclude(ad => ad.State)
                .OrderByDescending(p => p.CreatedTime)
                .ProjectTo<StoreBranchModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var listCountryId = branchManagements.Select(g => g.Address.CountryId).ToList();
            var listCountry = await _unitOfWork.Countries.Find(s => listCountryId.Any(Id => Id == s.Id)).ToListAsync();
            foreach (var branchManagement in branchManagements)
            {
                var country = listCountry.FirstOrDefault(s => s.Id == branchManagement.Address.CountryId);
                var isDefaultCountry = country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;

                branchManagement.AddressInfo = FormatAddress(branchManagement.Address, isDefaultCountry, country.Nicename);
            }

            if (!string.IsNullOrEmpty(request.KeySearch) && branchManagements != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                branchManagements = branchManagements.Where(g => g.Name.ToLower().Contains(keySearch)
                || g.PhoneNumber.ToLower().Contains(keySearch)
                || g.AddressInfo.ToLower().Contains(keySearch)).ToList();
            }

            var branchManagementsByPaging = branchManagements.ToPagination(request.PageNumber, request.PageSize);
            var branchManagementModels = _mapper.Map<IEnumerable<StoreBranchModel>>(branchManagementsByPaging.Result);

            var response = new GetBranchManagementsResponse()
            {
                BranchManagements = branchManagementModels,
                Total = branchManagements.Count
            };

            return response;
        }

        private static string FormatAddress(AddressModel address, bool branchManagement, string country)
        {
            List<string> addressComponents = new();
            if (address != null && !string.IsNullOrWhiteSpace(address.Address1))
            {
                addressComponents.Add(address.Address1);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.Address2) && !branchManagement)
            {
                addressComponents.Add(address.Address2);
            }
            if (address != null && address.Ward != null && !string.IsNullOrWhiteSpace(address.Ward.Name) && branchManagement)
            {
                addressComponents.Add(address.Ward.Name);
            }
            if (address != null && address.District != null && !string.IsNullOrWhiteSpace(address.District.Name) && branchManagement)
            {
                addressComponents.Add(address.District.Name);
            }
            if (address != null && address.City != null && !string.IsNullOrWhiteSpace(address.City.Name) && branchManagement)
            {
                addressComponents.Add(address.City.Name);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.CityTown) && !branchManagement)
            {
                addressComponents.Add(address.CityTown);
            }
            if (address != null && address.State != null && !string.IsNullOrWhiteSpace(address.State.Name) && !branchManagement)
            {
                addressComponents.Add(address.State.Name);
            }
            if (!string.IsNullOrWhiteSpace(country))
            {
                addressComponents.Add(country);
            }

            return string.Join(", ", addressComponents);
        }
    }
}

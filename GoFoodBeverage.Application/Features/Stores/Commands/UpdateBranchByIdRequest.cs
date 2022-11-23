using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class UpdateBranchByIdRequest : IRequest<bool>
    {
        public Guid BranchId { get; set; }

        public string BranchName { get; set; }

        public Guid? CountryId { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public Guid? StateId { get; set; }

        public Guid? CityId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? WardId { get; set; }

        public string CityTown { get; set; }

        public string PostalCode { get; set; }

        public LocationDto Location { get; set; }

        public class LocationDto
        {
            public double? Lat { get; set; }

            public double? Lng { get; set; }
        }
    }

    public class UpdateBranchByIdHandler : IRequestHandler<UpdateBranchByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdateBranchByIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateBranchByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);
            ThrowError.Against(store == null, "Cannot find store information, please authenticate before!");
            RequestValidation(request);
            var branch = await _unitOfWork.StoreBranches.Find(g => g.StoreId == loggedUser.StoreId && g.Id == request.BranchId && !g.IsDeleted)
                .Include(x => x.Address)
                .FirstOrDefaultAsync();

            if (request.BranchName.Trim().ToLower() != branch.Name.Trim().ToLower())
            {
                var storeBranchNameExisted = await _unitOfWork.StoreBranches
                    .Find(g => g.StoreId == loggedUser.StoreId && !g.IsDeleted && g.Name.Trim().ToLower() == request.BranchName.Trim().ToLower() )
                    .FirstOrDefaultAsync();
                ThrowError.Against(storeBranchNameExisted != null, new JObject()
                    {
                       { $"{nameof(request.BranchName)}",  "Name of branch has already existed"},
                    });
            }
            if (request.Email?.Trim().ToLower() != branch.Email?.Trim().ToLower())
            {
                var storeBrancheEmailExisted = await _unitOfWork.StoreBranches
                    .Find(g => g.StoreId == loggedUser.StoreId && g.Email.Trim().ToLower() == request.Email.Trim().ToLower() && !g.IsDeleted)
                    .FirstOrDefaultAsync();

                ThrowError.Against(storeBrancheEmailExisted != null, new JObject()
                    {
                       { $"{nameof(request.Email)}",  "Email of branch has already existed"},
                    });
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
                try
                {
                    var country = await _unitOfWork.Countries.GetCountryByIdAsync(request.CountryId.Value);
                    var isDefaultCountry = country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;
                    //Update Address
                    branch.Address.CountryId = request.CountryId;
                    branch.Address.StateId = request.StateId;
                    branch.Address.CityTown = request.CityTown;
                    branch.Address.CityId = request.CityId;
                    branch.Address.Address1 = request.Address1;
                    branch.Address.Address2 = request.Address2;
                    branch.Address.PostalCode = request.PostalCode;
                    if (request.Location != null)
                    {
                        branch.Address.Latitude = request.Location.Lat;
                        branch.Address.Longitude = request.Location.Lng;
                    }
                    if (isDefaultCountry)
                    {
                        branch.Address.DistrictId = request.DistrictId;
                        branch.Address.WardId = request.WardId;
                    }
                    await _unitOfWork.Addresses.UpdateAsync(branch.Address);

                    //Update Branch
                    branch.Name = request.BranchName;
                    branch.PhoneNumber = request.PhoneNumber;
                    branch.Email = request.Email;
                    await _unitOfWork.StoreBranches.UpdateAsync(branch);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(UpdateBranchByIdRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.BranchName), new JObject()
                  {
                      { $"{nameof(request.BranchName)}",  "Please enter name of branch."},
                  });
        }
    }
}

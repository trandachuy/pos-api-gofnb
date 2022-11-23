using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using Newtonsoft.Json.Linq;
using GoFoodBeverage.Application.Features.Stores.Queries;
using GoFoodBeverage.Common.Models.Base;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class CreateBranchRequest : IRequest<CreateBranchRequestResponse>
    {
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

    public class CreateBranchRequestResponse : ResponseModel
    {
    }

    public class CreateBranchManagementRequestHandler : IRequestHandler<CreateBranchRequest, CreateBranchRequestResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;
        private readonly IMediator _mediator;
        private readonly IDateTimeService _dateTimeService;

        public CreateBranchManagementRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService,
            IMediator mediator,
            IDateTimeService dateTimeService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
            _mediator = mediator;
            _dateTimeService = dateTimeService;
        }

        public async Task<CreateBranchRequestResponse> Handle(CreateBranchRequest request, CancellationToken cancellationToken)
        {
            var availableBranchNumberResult = await CheckAvailableBranchNumberAsync();
            if (availableBranchNumberResult != null) return availableBranchNumberResult; /// If AvailableBranchQuantity <= return

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);
            ThrowError.Against(store == null, "Cannot find store information, please authenticate before!");

            RequestValidation(request);

            var storeBranchNameExisted = await _unitOfWork.StoreBranches
                .Find(g => g.StoreId == loggedUser.StoreId && g.Name.Trim().ToLower() == request.BranchName.Trim().ToLower() && !g.IsDeleted)
                .FirstOrDefaultAsync();
            ThrowError.Against(storeBranchNameExisted != null, new JObject()
            {
                { $"{nameof(request.BranchName)}",  "Name of branch has already existed"},
            });

            var country = await _unitOfWork.Countries.GetCountryByIdAsync(request.CountryId.Value);
            var isDefaultCountry = country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;
            var newAddress = CreateStoreAddress(request, isDefaultCountry);
            var newStoreBranch = await CreateBranchAsync(request, store.Id, newAddress);

            _unitOfWork.StoreBranches.Add(newStoreBranch);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            var response = new CreateBranchRequestResponse()
            {
                Success = true
            };

            return response;
        }

        private static void RequestValidation(CreateBranchRequest request)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.BranchName), new JObject()
            {
                { $"{nameof(request.BranchName)}",  "Please enter name of branch"},
            });
        }

        private async  Task<StoreBranch> CreateBranchAsync(CreateBranchRequest request, Guid? storeId, Address newAddress)
        {
            var today = _dateTimeService.NowUtc;
            var activateStorePackage = await _unitOfWork.OrderPackages
             .GetAll()
             .AsNoTracking()
             .Where(op => op.StoreId == storeId &&
                          op.OrderPackageType == EnumOrderPackageType.StoreActivate &&
                          op.Status == EnumOrderPackageStatus.APPROVED.GetName() &&
                          op.IsActivated == true &&
                          op.ExpiredDate >= today)
             .OrderByDescending(op => op.LastModifiedDate)
             .Select(op => op.Package)
             .FirstOrDefaultAsync();

            var totalCurrentBranches = await _unitOfWork.StoreBranches
               .GetAll()
               .Where(b => b.StoreId == storeId && !b.IsDeleted)
               .AsNoTracking()
               .CountAsync();

            DateTime? branchPurchaseOrderPackageExpiredDate = null;
            if (totalCurrentBranches >= activateStorePackage.AvailableBranchNumber)
            {
                /// Get branch expired date from branch purchase order package
                branchPurchaseOrderPackageExpiredDate = await _unitOfWork.OrderPackages
                 .GetAll()
                 .AsNoTracking()
                 .Where(op => op.StoreId == storeId &&
                              op.OrderPackageType == EnumOrderPackageType.BranchPurchase &&
                              op.Status == EnumOrderPackageStatus.APPROVED.GetName() &&
                              op.IsActivated == true &&
                              op.ExpiredDate >= today)
                 .OrderByDescending(op => op.LastModifiedDate)
                 .Select(op => op.ExpiredDate)
                 .FirstOrDefaultAsync();
            }

            var newStoreBranch = new StoreBranch()
            {
                StoreId = (Guid)storeId,
                Name = request.BranchName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = newAddress,
                StatusId = (int)EnumStatus.Active,
                ExpiredDate = branchPurchaseOrderPackageExpiredDate
            };

            return newStoreBranch;
        }

        private static Address CreateStoreAddress(CreateBranchRequest request, bool isDefaultCountry)
        {
            var address = new Address()
            {
                CountryId = request.CountryId,
                StateId = request.StateId, // If the country another default (VN), value is NULL
                CityTown = request.CityTown, // If the country another default (VN), value is NULL
                CityId = request.CityId,
                Address1 = request.Address1,
                Address2 = request.Address2,
                PostalCode = request.PostalCode,
                Latitude = request.Location?.Lat,
                Longitude = request.Location?.Lng,
            };

            if (isDefaultCountry)
            {
                address.DistrictId = request.DistrictId;
                address.WardId = request.WardId;
            }

            return address;
        }

        private async Task<CreateBranchRequestResponse> CheckAvailableBranchNumberAsync()
        {
            var availableBranchNumberResponse = await _mediator.Send(new GetAvailableBranchQuantityRequest());
            if (availableBranchNumberResponse.AvailableBranchQuantity <= 0)
            {
                var createBranchFailResponse = new CreateBranchRequestResponse()
                {
                    Success = false,
                };

                return createBranchFailResponse;
            }

            return null;
        }
    }
}

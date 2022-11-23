using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Store;
using GoogleServices.Distance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoresByAddressRequest : IRequest<GetStoresByAddressResponse>
    {
        public DateTime? CurrentDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int? StoreType { get; set; }
    }

    public class GetStoresByAddressResponse
    {
        public IEnumerable<StoresModel> Stores { get; set; }
    }

    public class GetStoresByAddressRequestHandler : IRequestHandler<GetStoresByAddressRequest, GetStoresByAddressResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDistanceService _googleDistanceService;

        public GetStoresByAddressRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IGoogleDistanceService googleDistanceService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<GetStoresByAddressResponse> Handle(GetStoresByAddressRequest request, CancellationToken cancellationToken)
        {
            List<Tuple<Guid, Guid, double, double, double>> storeSeletions = new();
            var filterStore = request.StoreType.HasValue;
            var storeBranchesByAddress = _unitOfWork.StoreBranches
                .GetAllStoreBranches()
                .Where(item => !filterStore
                    || item.Store.BusinessAreaId == EnumBusinessArea.Both.ToGuid()
                    || item.Store.BusinessAreaId == ((EnumBusinessArea)request.StoreType).ToGuid())
                .Select(s => new { Id = s.Id, Lat = s.Address.Latitude, Lng = s.Address.Longitude, StoreId = s.StoreId })
                .ToList();

            foreach (var aBranch in storeBranchesByAddress)
            {
                if(aBranch.Lat.HasValue && aBranch.Lng.HasValue)
                {
                    var distance = MathHelpers.CalculateDistanceBetweenTwoPoints(request.Latitude, request.Longitude, aBranch.Lat.Value, aBranch.Lng.Value);

                    if (distance <= Common.Constants.DefaultConstants.DEFAULT_USER_STORE_DISTANCE)
                    {
                        var storeExist = storeSeletions.Find(s => s.Item1 == aBranch.StoreId);

                        if (storeExist != null && distance < storeExist.Item3)
                        {
                            storeSeletions.Remove(storeExist);
                            var storeSeletion = new Tuple<Guid, Guid, double, double, double>(aBranch.StoreId, aBranch.Id, distance, aBranch.Lat.Value, aBranch.Lng.Value);
                            storeSeletions.Add(storeSeletion);
                        }

                        if (storeExist == null)
                        {
                            var storeSeletion = new Tuple<Guid, Guid, double, double, double>(aBranch.StoreId, aBranch.Id, distance, aBranch.Lat.Value, aBranch.Lng.Value);
                            storeSeletions.Add(storeSeletion);
                        }
                    }
                }
            }

            var storeIds = storeSeletions.Select(s => s.Item1);
            var branchIds = storeSeletions.Select(s => s.Item2);

            var stores = await _unitOfWork.Stores
                .GetStoresByStoreIds(storeIds)
                .Select(s => new { s.Id, s.InitialStoreAccountId, s.Title, s.Logo })
                .ToListAsync(cancellationToken: cancellationToken);

            var storeBranchesResponse = await _unitOfWork.StoreBranches
                .GetAnyStoreBranchByIdAsync(branchIds)
                .ToListAsync(cancellationToken: cancellationToken);

            var initialStoreAccountIds = stores.Select(s => s.InitialStoreAccountId).ToList();

            var staffs = await _unitOfWork.Staffs.GetAll().Where(s => initialStoreAccountIds.Contains(s.AccountId))
                        .Select(s => new { s.Thumbnail, s.AccountId })
                        .ToListAsync();

            var promotions = await _unitOfWork.Promotions
                .GetAll()
                .Where(p => storeIds.Contains(p.StoreId) &&
                    p.IsStopped == false &&
                    (request.CurrentDate.HasValue && ((p.EndDate == null && p.StartDate != null) || (p.EndDate != null && p.EndDate >= request.CurrentDate))))
                .ToListAsync();

            var storesModel = new List<StoresModel>();

            // If the data has value, it will be opened from Recommend Store.
            // Promotions will display in real-time by the user's location.
            if (request.CurrentDate.HasValue)
            {
                foreach (var aStore in stores)
                {
                    var branch = storeBranchesResponse.FirstOrDefault(c => c.StoreId == aStore.Id);
                    var distanceLatLng = storeSeletions.FirstOrDefault(c => c.Item1 == aStore.Id);
                    var staff = staffs.FirstOrDefault(c => c.AccountId == aStore.InitialStoreAccountId);

                    // Count the total number of store promotions.
                    long totalPromotions = promotions.Count(p => p.StoreId == aStore.Id);

                    // Mask location data, will be removed in the future.
                    Random rd = new Random();

                    var distanceBetweenPoints = await _googleDistanceService.GetDistanceBetweenPointsAsync(request.Latitude, request.Longitude, distanceLatLng.Item4, distanceLatLng.Item5, cancellationToken);

                    var storeBranch = new StoresModel.BranchModel()
                    {
                        Id = branch.Id,
                        Name = branch.Name
                    };


                    var storeModel = new StoresModel()
                    {
                        Id = aStore.Id,
                        Title = aStore.Title,
                        Logo = aStore?.Logo,
                        IsPromotion = totalPromotions > 0,
                        StoreBranches = storeBranch,
                        Distance = distanceBetweenPoints,
                        // Rating = the value will be used in the future.
                    };
                    storesModel.Add(storeModel);
                }
            }

            var storesByPromotion = storesModel.Where(s => s.IsPromotion == true).OrderBy(s => s.Distance);
            var storesByLocation = storesModel.Where(s => s.IsPromotion == false).OrderBy(s => s.Distance);
            storesModel = storesByPromotion.Concat(storesByLocation).ToList();

            var response = new GetStoresByAddressResponse()
            {
                Stores = storesModel
            };

            return response;
        }
    }
}

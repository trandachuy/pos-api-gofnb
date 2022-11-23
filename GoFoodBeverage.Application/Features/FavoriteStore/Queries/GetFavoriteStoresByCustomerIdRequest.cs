using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleServices.Distance;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.FavoriteStore;
using GoFoodBeverage.Common.Helpers;

namespace GoFoodBeverage.Application.Features.FavoriteStores.Queries
{
    public class GetFavoriteStoresByCustomerIdRequest : IRequest<GetFavoriteStoresByCustomerIdResponse>
    {
        public Guid CustomerId { get; set; }

        public DateTime CurrentDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class GetFavoriteStoresByCustomerIdResponse
    {
        public IEnumerable<FavoriteStoreModel> FavoriteStores { get; set; }
    }

    public class GetFavoriteStoresByCustomerIdRequestHandler : IRequestHandler<GetFavoriteStoresByCustomerIdRequest, GetFavoriteStoresByCustomerIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDistanceService _googleDistanceService;

        public GetFavoriteStoresByCustomerIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IGoogleDistanceService googleDistanceService
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<GetFavoriteStoresByCustomerIdResponse> Handle(GetFavoriteStoresByCustomerIdRequest request, CancellationToken cancellationToken)
        {

            var favoriteStores = await _unitOfWork.FavoriteStores
                .GetAll()
                .Where(o => o.AccountId == request.CustomerId)
                .Include(o => o.Store).ThenInclude(b => b.StoreBranches.Skip(0).Take(1))
                .Select(fs => new {
                    StoreId = fs.Store.Id,
                    StoreTitle  = fs.Store.Title,
                    Logo = fs.Store.Logo,
                    InitialStoreAccountId  = fs.Store.InitialStoreAccountId,
                    StoreBranches  = fs.Store.StoreBranches.FirstOrDefault()
                })
                .ToListAsync();

            var initialStoreAccountIds = favoriteStores.Select(s => s.InitialStoreAccountId).ToList();

            var staffs = await _unitOfWork.Staffs.GetAllStaffByInitialStoreAccounts(initialStoreAccountIds)
                        .Select(s => new { s.Thumbnail, s.AccountId})
                        .ToListAsync();

            var storeIds = favoriteStores.Select(fs => fs.StoreId).ToList();
            var promotions = await _unitOfWork.Promotions
                .GetAll()
                .Where(p => storeIds.Contains(p.StoreId) &&
                    p.IsStopped == false &&
                    ((p.EndDate == null && p.StartDate != null) || (p.EndDate != null && p.EndDate >= request.CurrentDate)))
                .ToListAsync();

            var storeBranchesByAddress = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreIds(storeIds)
                .Select(s => new { Id = s.Id, Lat = s.Address.Latitude, Lng = s.Address.Longitude, StoreId = s.StoreId })
                .ToListAsync(cancellationToken: cancellationToken);
            List<Tuple<Guid, Guid, double, double, double>> storeSeletions = new();
            foreach (var aBranch in storeBranchesByAddress)
            {
                if (aBranch.Lat.HasValue && aBranch.Lng.HasValue)
                {
                    var distance = MathHelpers.CalculateDistanceBetweenTwoPoints(request.Latitude, request.Longitude, aBranch.Lat.Value, aBranch.Lng.Value);

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


            var favoriteStoresResponse = new List<FavoriteStoreModel>();
            foreach (var favoriteStore in favoriteStores)
            {
                var distanceBetweenPoints = 0;
                var distanceLatLng = storeSeletions.FirstOrDefault(c => c.Item1 == favoriteStore.StoreId);
                if(distanceLatLng != null)
                {
                    distanceBetweenPoints = await _googleDistanceService.GetDistanceBetweenPointsAsync(request.Latitude, request.Longitude, distanceLatLng.Item4, distanceLatLng.Item5, cancellationToken);
                }

                var storeBranches = _mapper.Map<FavoriteStoreModel.BranchModel>(favoriteStore.StoreBranches);

                var staff = staffs.FirstOrDefault(c => c.AccountId == favoriteStore.InitialStoreAccountId);

                // Count the total number of store promotions.
                long totalPromotions = promotions.Count(p => p.StoreId == favoriteStore.StoreId);

                // Mask location data, will be removed in the future.
                Random rd = new Random();
                var favoriteStoreResponse = new FavoriteStoreModel()
                {
                    StoreId = favoriteStore.StoreId,
                    StoreTitle = favoriteStore.StoreTitle,
                    StoreThumbnail = favoriteStore.Logo,
                    StoreBranches = storeBranches,
                    IsPromotion = totalPromotions > 0,
                    Distance = distanceBetweenPoints,
                    Rating = Math.Round(rd.Next(0, 4) + rd.NextDouble(), 1)
                };
                favoriteStoresResponse.Add(favoriteStoreResponse);
            }

            return new GetFavoriteStoresByCustomerIdResponse
            {
                FavoriteStores = favoriteStoresResponse
            };
        }
    }
}

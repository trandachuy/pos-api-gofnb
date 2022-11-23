using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleServices.Distance;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Models.DeliveryMethod;
using GoFoodBeverage.Delivery.Ahamove.Model;

namespace GoFoodBeverage.Application.Features.DeliveryMethods.Queries
{
    public class GetEstimateFeeDeliveryMethodsByAddressRequest : IRequest<GetEstimateFeeDeliveryConfigsByAddressResponse>
    {
        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }

        public AhamoveAddressDto ReceiverAddress { get; set; }

        public class AhamoveAddressDto
        {
            public string Address { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }
        }
    }

    public class GetEstimateFeeDeliveryConfigsByAddressResponse
    {
        public IEnumerable<EstimateFeeDeliveryMethodsModel> FeeDeliveryConfigs { get; set; }
    }

    public class GetEstimateFeeDeliveryConfigsByAddressRequestHandler : IRequestHandler<GetEstimateFeeDeliveryMethodsByAddressRequest, GetEstimateFeeDeliveryConfigsByAddressResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAhamoveService _ahamoveService;
        private readonly IGoogleDistanceService _googleDistanceService;

        public GetEstimateFeeDeliveryConfigsByAddressRequestHandler(
            IUnitOfWork unitOfWork,
            IAhamoveService ahamoveService,
            IGoogleDistanceService googleDistanceService)
        {
            _unitOfWork = unitOfWork;
            _ahamoveService = ahamoveService;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<GetEstimateFeeDeliveryConfigsByAddressResponse> Handle(GetEstimateFeeDeliveryMethodsByAddressRequest request, CancellationToken cancellationToken)
        {
            var feeDeliveryConfigs = new List<EstimateFeeDeliveryMethodsModel>();
            var deliveryConfig = new DeliveryConfig();

            decimal estimateSelfDeliveryOrderFee = 0;
            decimal estimateAhaMoveDeliveryOrderFee = 0;

            var deliveryMethods = await _unitOfWork.DeliveryConfigs
                .GetAll()
                .Include(dvcp => dvcp.DeliveryConfigPricings)
                .Where(dvc => dvc.StoreId == request.StoreId && dvc.IsActivated == true)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var branch = await _unitOfWork.StoreBranches
                .GetStoreBranchByStoreIdAndBranchIdAsync(request.StoreId, request.BranchId)
                .Select(b => new { b.Address, b.Address.Country, b.Address.Latitude, b.Address.Longitude, b.Address.Address1, b.Address.Address2, b.Address.City, b.Address.District, b.Address.Ward })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var senderAddressDetail = FormatAddress(branch.Address);

            var ahaMoveDeliveryMethod = deliveryMethods.FirstOrDefault(dvm => dvm.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove);
            var selfDeliveryMethod = deliveryMethods.FirstOrDefault(dvm => dvm.DeliveryMethodEnumId == EnumDeliveryMethod.SelfDelivery);

            if (ahaMoveDeliveryMethod != null)
            {
                estimateAhaMoveDeliveryOrderFee = await GetEstimateAhaMoveOrderFeeAddressAsync(ahaMoveDeliveryMethod, senderAddressDetail, branch.Latitude, branch.Longitude, request.ReceiverAddress.Address, request.ReceiverAddress.Lat, request.ReceiverAddress.Lng);
            }

            if (selfDeliveryMethod != null)
            {
                estimateSelfDeliveryOrderFee = await GetEstimateSelfDeliveryOrderFeeAddressAsync(selfDeliveryMethod, branch.Latitude, branch.Longitude, request.ReceiverAddress.Lat, request.ReceiverAddress.Lng, cancellationToken);
            }

            foreach (var deliveryMethod in deliveryMethods)
            {
                var isSelfDelivery = deliveryMethod.DeliveryMethodEnumId == EnumDeliveryMethod.SelfDelivery;
                var isAhaMoveDelivery = deliveryMethod.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove;

                if ((isSelfDelivery && deliveryConfig != null) || isAhaMoveDelivery)
                {
                    var feeDeliveryConfig = new EstimateFeeDeliveryMethodsModel()
                    {
                        DeliveryMethodId = deliveryMethod.DeliveryMethodId.Value,
                        EnumId = deliveryMethod.DeliveryMethodEnumId,
                        FeeValue = deliveryMethod.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove ? estimateAhaMoveDeliveryOrderFee : estimateSelfDeliveryOrderFee
                    };
                    feeDeliveryConfigs.Add(feeDeliveryConfig);
                }
            }

            var dataToResponse = new GetEstimateFeeDeliveryConfigsByAddressResponse()
            {
                FeeDeliveryConfigs = feeDeliveryConfigs
            };

            return dataToResponse;
        }

        private static string FormatAddress(Address address)
        {
            var isDefaultCountry = address.Country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;

            List<string> addressComponents = new();
            if (address != null && !string.IsNullOrWhiteSpace(address.Address1))
            {
                addressComponents.Add(address.Address1);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.Address2) && !isDefaultCountry)
            {
                addressComponents.Add(address.Address2);
            }
            if (address != null && address.Ward != null && !string.IsNullOrWhiteSpace(address.Ward.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.Ward.Name);
            }
            if (address != null && address.District != null && !string.IsNullOrWhiteSpace(address.District.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.District.Name);
            }
            if (address != null && address.City != null && !string.IsNullOrWhiteSpace(address.City.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.City.Name);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.CityTown) && !isDefaultCountry)
            {
                addressComponents.Add(address.CityTown);
            }
            if (address != null && address.State != null && !string.IsNullOrWhiteSpace(address.State.Name) && !isDefaultCountry)
            {
                addressComponents.Add(address.State.Name);
            }
            if (!string.IsNullOrWhiteSpace(address.Country.Nicename))
            {
                addressComponents.Add(address.Country.Nicename);
            }

            return string.Join(", ", addressComponents);
        }

        private async Task<decimal> GetEstimateAhaMoveOrderFeeAddressAsync(DeliveryConfig ahamoveConfig, string senderAddress, double? senderLatitude, double? senderLongitude, string receiverAddress, double receiverLatitude, double receiverLongitude)
        {
            decimal ahaMoveOrderFee = 0;

            var senderAddressDetail = new EstimateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Address = senderAddress,
                Lat = senderLatitude.HasValue ? senderLatitude.Value : 0,
                Lng = senderLongitude.HasValue ? senderLongitude.Value : 0,
            };

            var receiverAddressDetail = new EstimateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Address = receiverAddress,
                Lat = receiverLatitude,
                Lng = receiverLongitude,
            };

            var estimateOrderAhamoveRequest = new EstimateOrderAhamoveRequestModel()
            {
                SenderAddress = senderAddressDetail,
                ReceiverAddress = receiverAddressDetail
            };

            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahamoveConfig.PhoneNumber,
                Name = ahamoveConfig.Name,
                ApiKey = ahamoveConfig.ApiKey,
                Address = ahamoveConfig.Address,
            };

            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);

            if(infoToken != null && infoToken.Token != null)
            {
                var estimateOrderFeeResponse = await _ahamoveService.EstimateOrderFee(infoToken.Token, estimateOrderAhamoveRequest);

                if (estimateOrderFeeResponse != null)
                {
                    ahaMoveOrderFee = estimateOrderFeeResponse.TotalPrice;
                }
            }

            return ahaMoveOrderFee;
        }

        private async Task<decimal> GetEstimateSelfDeliveryOrderFeeAddressAsync(DeliveryConfig deliveryConfig, double? senderLatitude, double? senderLongitude, double receiverAddressLat, double receiverAddressLng, CancellationToken cancellationToken)
        {
            decimal deliveryFee = 0;

            var senderLat = senderLatitude.HasValue ? senderLatitude.Value : 0;
            var senderLng = senderLongitude.HasValue ? senderLongitude.Value : 0;

            var distanceBetweenPoints = await _googleDistanceService.GetDistanceBetweenPointsAsync(receiverAddressLat, receiverAddressLng, senderLat, senderLng, cancellationToken);

            if (deliveryConfig != null)
            {
                if ((bool)deliveryConfig.IsFixedFee)
                {
                    deliveryFee = deliveryConfig.FeeValue.Value;
                }
                else if (deliveryConfig.DeliveryConfigPricings.Count() > 0)
                {
                    var deliveryConfigPrice = deliveryConfig.DeliveryConfigPricings.FirstOrDefault(dvcp => dvcp.FromDistance * 1000 < distanceBetweenPoints && distanceBetweenPoints <= dvcp.ToDistance * 1000);

                    if (deliveryConfigPrice != null)
                    {
                        deliveryFee = deliveryConfigPrice.FeeValue;
                    }
                    else
                    {
                        var configPrice = deliveryConfig.DeliveryConfigPricings.OrderByDescending(dvp => dvp.ToDistance).FirstOrDefault();
                        deliveryFee = configPrice.FeeValue;
                    }
                }
            }

            return deliveryFee;
        }
    }
}

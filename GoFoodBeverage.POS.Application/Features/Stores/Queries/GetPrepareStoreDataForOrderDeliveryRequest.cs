using System.Linq;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.POS.Models.DeliveryMethod;
using GoFoodBeverage.POS.Models.Store;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Delivery.Ahamove;

namespace GoFoodBeverage.POS.Application.Features.Stores.Queries
{
    public class GetPrepareStoreDataForOrderDeliveryRequest : IRequest<GetPrepareStoreDataForOrderDeliveryResponse>
    {
    }

    public class GetPrepareStoreDataForOrderDeliveryResponse
    {
        public IEnumerable<DeliveryMethodModel> DeliveryMethods { get; set; }

        public StoreBankAccountModel StoreBankAccount { get; set; }

        public StoreInformationModel StoreInformation { get; set; }
    }

    public class GetPrepareStoreDataForOrderDeliveryRequestHandler : IRequestHandler<GetPrepareStoreDataForOrderDeliveryRequest, GetPrepareStoreDataForOrderDeliveryResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAhamoveService _ahamoveService;

        public GetPrepareStoreDataForOrderDeliveryRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAhamoveService ahamoveService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ahamoveService = ahamoveService;
        }

        public async Task<GetPrepareStoreDataForOrderDeliveryResponse> Handle(GetPrepareStoreDataForOrderDeliveryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            /// Get delivery methods
            var deliveryMethods = await _unitOfWork.DeliveryConfigs
                .Find(x => x.StoreId == loggedUser.StoreId && x.IsActivated == true)
                .OrderBy(x => x.DeliveryMethodEnumId)
                .Select(x => new
                {
                    Id = x.DeliveryMethodId.Value,
                    EnumId = x.DeliveryMethodEnumId,
                    Name = x.DeliveryMethodEnumId.GetName(),
                    ApiKey = x.ApiKey,
                    StorePhone = x.PhoneNumber,
                    StoreName = x.Name,
                    StoreAddress = x.Address
                })
                .ToListAsync(cancellationToken: cancellationToken);

            bool isAhamoveActive = false;
            var ahaMoveDeliveryMethod = deliveryMethods.FirstOrDefault(dvm => dvm.EnumId == EnumDeliveryMethod.AhaMove);
            if(ahaMoveDeliveryMethod != null)
            {
                isAhamoveActive = await CheckAhaMoveConfigActiveAsync(ahaMoveDeliveryMethod.ApiKey, ahaMoveDeliveryMethod.StorePhone, ahaMoveDeliveryMethod.StoreName, ahaMoveDeliveryMethod.StoreAddress);
            }

            var deliveryMethodsModel = new List<DeliveryMethodModel>();
            foreach (var deliveryMethod in deliveryMethods)
            {
                if (deliveryMethod.EnumId == EnumDeliveryMethod.SelfDelivery || (deliveryMethod.EnumId == EnumDeliveryMethod.AhaMove && isAhamoveActive))
                {
                    var deliveryMethodModel = new DeliveryMethodModel()
                    {
                        Id = deliveryMethod.Id,
                        EnumId = deliveryMethod.EnumId,
                        Name = deliveryMethod.Name,
                    };
                    deliveryMethodsModel.Add(deliveryMethodModel);
                }
            }

            /// Get bank transfer information
            var storeBankAccount = await _unitOfWork.StoreBankAccounts.GetStoreBankAccountByStoreIdAsync(loggedUser.StoreId);
            var storeBankAccountModel = _mapper.Map<StoreBankAccountModel>(storeBankAccount);

            /// Get other store information
            var storeBranchInfo = await _unitOfWork.StoreBranches.GetBranchAddressByStoreIdAndBranchIdAsync(loggedUser.StoreId, loggedUser.BranchId);

            string branchAddress = string.Format("{0}, {1}, {2}, {3}, {4}",
                storeBranchInfo?.Address?.Address1,
                storeBranchInfo?.Address?.Ward?.Name,
                storeBranchInfo?.Address?.District?.Name,
                storeBranchInfo?.Address?.City?.Name,
                storeBranchInfo?.Address?.Country?.Name);

            var storeInfo = await _unitOfWork.Stores
                .Find(s => s.Id == loggedUser.StoreId)
                .Select(s => new StoreInformationModel()
                {
                    Logo = s.Logo,
                    IsStoreHasKitchen = s.IsStoreHasKitchen,
                    IsAutoPrintStamp = s.IsAutoPrintStamp,
                    IsPaymentLater = s.IsPaymentLater,
                    IsCheckProductSell = s.IsCheckProductSell,
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var storeBranchInfoModel = new StoreInformationModel
            {
                PhoneCode = storeBranchInfo?.Address?.Country?.Phonecode,
                CountryIso = storeBranchInfo?.Address?.Country?.Iso,
                CountryName = storeBranchInfo?.Address?.Country?.Name,
                BranchAddress = branchAddress,
                Logo = storeInfo.Logo,
                IsStoreHasKitchen = storeInfo.IsStoreHasKitchen,
                IsAutoPrintStamp = storeInfo.IsAutoPrintStamp,
                IsPaymentLater = storeInfo.IsPaymentLater,
                IsCheckProductSell = storeInfo.IsCheckProductSell,
            };

            /// Get branch location
            var branchLocation = new StoreInformationModel.LocationDto();
            if (storeBranchInfo?.Address?.Latitude != null && storeBranchInfo?.Address?.Longitude != null)
            {
                branchLocation.Lat = storeBranchInfo?.Address?.Latitude;
                branchLocation.Lng = storeBranchInfo?.Address?.Longitude;
                storeBranchInfoModel.BranchLocation = branchLocation;
            }

            var response = new GetPrepareStoreDataForOrderDeliveryResponse()
            {
                DeliveryMethods = deliveryMethodsModel,
                StoreBankAccount = storeBankAccountModel,
                StoreInformation = storeBranchInfoModel,
            };

            return response;
        }

        private async Task<bool> CheckAhaMoveConfigActiveAsync(string apiKey, string phone, string name, string address)
        {
            bool isAhamoveActive = false;
            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = phone,
                Name = name,
                ApiKey = apiKey,
                Address = address,
            };

            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);

            if (infoToken != null && infoToken.Token != null)
            {
                isAhamoveActive = true;
            }

            return isAhamoveActive;
        }
    }
}

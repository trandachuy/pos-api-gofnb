using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.DeliveryConfig;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Delivery.Ahamove.Model;

namespace GoFoodBeverage.Application.Features.DeliveryConfigs.Commands
{
    public class UpdateAhaMoveConfigRequest : IRequest<bool>
    {
        public AhaMoveConfigModel AhaMoveConfig { get; set; }
    }

    public class UpdateAhaMoveConfigRequestHandler : IRequestHandler<UpdateAhaMoveConfigRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;
        private readonly IAhamoveService _ahamoveService;

        public UpdateAhaMoveConfigRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService,
            IAhamoveService ahamoveService
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
            _ahamoveService = ahamoveService;
        }

        public async Task<bool> Handle(UpdateAhaMoveConfigRequest request, CancellationToken cancellationToken)
        {
            var isAhamoveKeyActive = await CheckAhaMoveKey(request.AhaMoveConfig);

            if(isAhamoveKeyActive)
            {
                var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

                var ahaMoveConfig = await _unitOfWork.DeliveryConfigs
                       .GetAhaMoveConfigByDeliveryMethodIdAsync(request.AhaMoveConfig.DeliveryMethodId, loggedUser.StoreId.Value);

                if (ahaMoveConfig == null)
                {
                    var newDeliveryConfig = CreateAhaMoveConfig(request.AhaMoveConfig, loggedUser.StoreId.Value, loggedUser.AccountId.Value);
                    await _unitOfWork.DeliveryConfigs.AddAsync(newDeliveryConfig);
                }
                else
                {
                    var modifiedDeliveryConfig = UpdateAhaMoveConfig(ahaMoveConfig, request.AhaMoveConfig, loggedUser.AccountId.Value);
                    await _unitOfWork.DeliveryConfigs.UpdateAsync(modifiedDeliveryConfig);
                }

                await _unitOfWork.SaveChangesAsync();
                await _userActivityService.LogAsync(request);
            }

            return isAhamoveKeyActive;
        }

        public static DeliveryConfig CreateAhaMoveConfig(AhaMoveConfigModel request, Guid storeId, Guid accountId)
        {
            var newAhaMoveConfig = new DeliveryConfig()
            {
                StoreId = storeId,
                DeliveryMethodEnumId = EnumDeliveryMethod.AhaMove,
                DeliveryMethodId = request.DeliveryMethodId,
                ApiKey = request.ApiKey,
                PhoneNumber = request.PhoneNumber,
                Name = request.Name,
                Address = request.Address,
                CreatedUser = accountId,
                IsActivated = true
            };

            return newAhaMoveConfig;
        }

        private static DeliveryConfig UpdateAhaMoveConfig(DeliveryConfig ahaMoveConfig, AhaMoveConfigModel request, Guid accountId)
        {
            ahaMoveConfig.ApiKey = request.ApiKey;
            ahaMoveConfig.PhoneNumber = request.PhoneNumber;
            ahaMoveConfig.Name = request.Name;
            ahaMoveConfig.Address = request.Address;
            ahaMoveConfig.LastSavedUser = accountId;
            ahaMoveConfig.IsActivated = true;

            return ahaMoveConfig;
        }

        private async Task<bool> CheckAhaMoveKey(AhaMoveConfigModel ahaMoveConfig)
        {
            bool isAhamoveKeyActive = false;

            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahaMoveConfig.PhoneNumber,
                Name = ahaMoveConfig.Name,
                ApiKey = ahaMoveConfig.ApiKey,
                Address = ahaMoveConfig.Address,
            };
            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);

            if(infoToken != null && infoToken.Token != null)
            {
                isAhamoveKeyActive = true;
            }

            return isAhamoveKeyActive;
        }
    }
}

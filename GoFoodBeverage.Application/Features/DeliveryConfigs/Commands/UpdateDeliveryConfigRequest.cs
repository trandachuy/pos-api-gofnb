using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

using GoFoodBeverage.Models.DeliveryConfig;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.DeliveryConfigs.Commands
{
    public class UpdateDeliveryConfigRequest : IRequest<bool>
    {
        public UpdateDeliveryConfigModel DeliveryConfig { get; set; }
    }

    public class UpdateDeliveryConfigRequestHandler : IRequestHandler<UpdateDeliveryConfigRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdateDeliveryConfigRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateDeliveryConfigRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var deliveryConfig = await _unitOfWork.DeliveryConfigs
                .GetDeliveryConfigByDeliveryMethodIdAsync(request.DeliveryConfig.DeliveryMethodId, loggedUser.StoreId.Value);

            if(deliveryConfig == null)
            {
                var newDeliveryConfig = CreateDeliveryConfig(request.DeliveryConfig, loggedUser.StoreId.Value, loggedUser.AccountId.Value);
                await _unitOfWork.DeliveryConfigs.AddAsync(newDeliveryConfig);
            } else
            {
                var modifiedDeliveryConfig = await UpdateDeliveryConfig(deliveryConfig, request.DeliveryConfig, loggedUser.AccountId.Value, loggedUser.StoreId.Value);
                await _unitOfWork.DeliveryConfigs.UpdateAsync(modifiedDeliveryConfig);
            }

            await _unitOfWork.SaveChangesAsync();
            await _userActivityService.LogAsync(request);

            return true;
        }

        public static DeliveryConfig CreateDeliveryConfig(UpdateDeliveryConfigModel request, Guid storeId, Guid accountId)
        {
            var newDeliveryConfig = new DeliveryConfig()
            {
                StoreId = storeId,
                DeliveryMethodEnumId = EnumDeliveryMethod.SelfDelivery,
                DeliveryMethodId = request.DeliveryMethodId,
                IsFixedFee = request.IsFixedFee,
                CreatedUser = accountId,
                IsActivated = true
            };

            if (request.IsFixedFee)
            {
                newDeliveryConfig.FeeValue = request.FeeValue;
            } else
            {
                newDeliveryConfig.DeliveryConfigPricings = new List<DeliveryConfigPricing>();
                int position = 0;
                foreach (var deliveryConfigPricing in request.DeliveryConfigPricings)
                {
                    position += 1;
                    var newDeliveryConfigPricing = new DeliveryConfigPricing()
                    {
                        DeliveryConfigId = newDeliveryConfig.Id,
                        Position = position,
                        FromDistance = deliveryConfigPricing.FromDistance,
                        ToDistance = deliveryConfigPricing.ToDistance,
                        FeeValue = deliveryConfigPricing.FeeValue,
                        CreatedUser = accountId,
                        StoreId = storeId,
                    };
                    newDeliveryConfig.DeliveryConfigPricings.Add(newDeliveryConfigPricing);
                }
            }

            return newDeliveryConfig;
        }

        private async Task<DeliveryConfig> UpdateDeliveryConfig(DeliveryConfig deliveryConfig, UpdateDeliveryConfigModel request, Guid accountId, Guid? storeId)
        {
            deliveryConfig.IsFixedFee = request.IsFixedFee;
            deliveryConfig.LastSavedUser = accountId;
            deliveryConfig.IsActivated = true;

            if (request.IsFixedFee)
            {
                deliveryConfig.FeeValue = request.FeeValue;
                var deleteDeliveryConfigPricings = deliveryConfig.DeliveryConfigPricings;
                if (deleteDeliveryConfigPricings.Any())
                {
                    await _unitOfWork.DeliveryConfigPricings.RemoveRangeAsync(deleteDeliveryConfigPricings);
                }
                
            } else
            {
                deliveryConfig.FeeValue = 0;
                var currentDeliveryConfigPricings = deliveryConfig.DeliveryConfigPricings.ToList();
                var newDeliveryConfigPricings = new List<DeliveryConfigPricing>();
                var updateDeliveryConfigPricings = new List<DeliveryConfigPricing>();

                if (request.DeliveryConfigPricings != null && request.DeliveryConfigPricings.Any())
                {
                    var deleteDeliveryConfigPricings = currentDeliveryConfigPricings
                        .Where(x => x.DeliveryConfigId == deliveryConfig.Id && !request.DeliveryConfigPricings.Select(ol => ol.Id).Contains(x.Id));
                    if (deleteDeliveryConfigPricings.Any())
                    {
                        _unitOfWork.DeliveryConfigPricings.RemoveRange(deleteDeliveryConfigPricings);
                    }
                    int position = 0;
                    int prevToDistance = 0;
                    foreach (var deliveryConfigPricing in request.DeliveryConfigPricings)
                    {
                        position += 1;
                        var deliveryConfigPricingExisted = currentDeliveryConfigPricings.FirstOrDefault(x => x.Id == deliveryConfigPricing.Id);
                        if (deliveryConfigPricingExisted == null)
                        {
                            var newDeliveryConfigPricing = new DeliveryConfigPricing()
                            {
                                DeliveryConfigId = deliveryConfig.Id,
                                Position = position,
                                FromDistance = deliveryConfigPricing.FromDistance,
                                ToDistance = deliveryConfigPricing.ToDistance,
                                FeeValue = deliveryConfigPricing.FeeValue,
                                CreatedUser = accountId,
                                StoreId = storeId
                        };
                            newDeliveryConfigPricings.Add(newDeliveryConfigPricing);
                        }
                        else
                        {
                            if (position > 1)
                                deliveryConfigPricingExisted.FromDistance = prevToDistance;
                            deliveryConfigPricingExisted.ToDistance = deliveryConfigPricing.ToDistance;
                            deliveryConfigPricingExisted.FeeValue = deliveryConfigPricing.FeeValue;
                            deliveryConfigPricingExisted.Position = position;
                            deliveryConfigPricingExisted.LastSavedUser = accountId;
                            updateDeliveryConfigPricings.Add(deliveryConfigPricingExisted);
                        }
                        prevToDistance = deliveryConfigPricing.ToDistance;
                    }

                    _unitOfWork.DeliveryConfigPricings.AddRange(newDeliveryConfigPricings);
                    _unitOfWork.DeliveryConfigPricings.UpdateRange(updateDeliveryConfigPricings);

                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return deliveryConfig;
        }
    }
}

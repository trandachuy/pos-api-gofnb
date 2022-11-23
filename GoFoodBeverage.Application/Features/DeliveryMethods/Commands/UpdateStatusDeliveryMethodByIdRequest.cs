using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.DeliveryMethods.Commands
{
    public class UpdateStatusDeliveryMethodByIdRequest : IRequest<bool>
    {
        public EnumDeliveryMethod Id { get; set; }

        public bool IsActivated { get; set; }
    }

    public class UpdateStatusDeliveryMethodByIdRequestHandler : IRequestHandler<UpdateStatusDeliveryMethodByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateStatusDeliveryMethodByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateStatusDeliveryMethodByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var deliveryMethod = await _unitOfWork.DeliveryMethods.Find(p => p.EnumId == request.Id).FirstOrDefaultAsync();

            var deliveryConfig = await _unitOfWork.DeliveryConfigs.Find(p => p.StoreId == loggedUser.StoreId && p.DeliveryMethodId == deliveryMethod.Id).FirstOrDefaultAsync();

            if(deliveryConfig != null)
            {
                var modifiedDeliveryConfig = UpdateStatusDeliveryConfig(deliveryConfig, loggedUser.AccountId.Value, request.IsActivated);
                await _unitOfWork.DeliveryConfigs.UpdateAsync(modifiedDeliveryConfig);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public static DeliveryConfig UpdateStatusDeliveryConfig(DeliveryConfig deliveryConfig, Guid accountId, bool isActivated)
        {
            deliveryConfig.IsActivated = isActivated;
            deliveryConfig.LastSavedUser = accountId;

            return deliveryConfig;
        }
    }
}

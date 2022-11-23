using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class ModifyLoyaltyPointRequest : IRequest<bool>
    {
        public bool IsActivated { get; set; }

        public bool IsExpiryDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? EarningPointExchangeValue { get; set; }

        public decimal? RedeemPointExchangeValue { get; set; }

        public bool IsExpiryMembershipDate { get; set; }

        public DateTime? ExpiryMembershipDate { get; set; }
    }

    public class ModifyLoyaltyPointHandler : IRequestHandler<ModifyLoyaltyPointRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public ModifyLoyaltyPointHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(ModifyLoyaltyPointRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            var storeConfiguration = await _unitOfWork.LoyaltyPointsConfigs.GetLoyaltyPointConfigByStoreIdAsync(loggerUser.StoreId.Value)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (storeConfiguration == null)
            {
                var configuration = CreateNewConfiguration(request, loggerUser.AccountId.Value, loggerUser.StoreId.Value);
                await _unitOfWork.LoyaltyPointsConfigs.AddAsync(configuration);

                await _mediator.Send(new CreateStaffActivitiesRequest()
                {
                    ActionGroup = EnumActionGroup.PointConfiguration,
                    ActionType = EnumActionType.Created,
                    ObjectId = configuration.Id,
                });
            } else
            {
                var configuration = UpdateConfiguration(storeConfiguration, request, loggerUser.AccountId.Value);
                await _unitOfWork.LoyaltyPointsConfigs.UpdateAsync(configuration);

                await _mediator.Send(new CreateStaffActivitiesRequest()
                {
                    ActionGroup = EnumActionGroup.PointConfiguration,
                    ActionType = EnumActionType.Edited,
                    ObjectId = configuration.Id,
                });
            }
            return true;
        }

        private LoyaltyPointConfig CreateNewConfiguration(ModifyLoyaltyPointRequest request, Guid loggedUserId, Guid storeId)
        {
            var configuration = new LoyaltyPointConfig()
            {
                StoreId = storeId,
                IsActivated = request.IsActivated,
                IsExpiryDate = request.IsExpiryDate,
                ExpiryDate = request.IsExpiryDate ? request.ExpiryDate.Value.EndOfDay().ToUniversalTime() : null,
                EarningPointExchangeValue = request.EarningPointExchangeValue,
                RedeemPointExchangeValue = request.RedeemPointExchangeValue,
                IsExpiryMembershipDate = request.IsExpiryMembershipDate,
                ExpiryMembershipDate = request.IsExpiryMembershipDate? request.ExpiryMembershipDate.Value.EndOfDay().ToUniversalTime() : null,
                CreatedUser = loggedUserId,
            };
            return configuration;
        }

        private LoyaltyPointConfig UpdateConfiguration(LoyaltyPointConfig currentData, ModifyLoyaltyPointRequest changeRequest, Guid loggedUserId)
        {
            currentData.IsActivated = changeRequest.IsActivated;
            currentData.IsExpiryDate = changeRequest.IsExpiryDate;
            currentData.ExpiryDate = changeRequest.IsExpiryDate? changeRequest.ExpiryDate.Value.EndOfDay().ToUniversalTime() : null;
            currentData.EarningPointExchangeValue = changeRequest.EarningPointExchangeValue;
            currentData.RedeemPointExchangeValue = changeRequest.RedeemPointExchangeValue;
            currentData.IsExpiryMembershipDate = changeRequest.IsExpiryMembershipDate;
            currentData.ExpiryMembershipDate = changeRequest.IsExpiryMembershipDate ? changeRequest.ExpiryMembershipDate.Value.EndOfDay().ToUniversalTime() : null;
            currentData.LastSavedUser = loggedUserId;
            return currentData;
        }
    }
}

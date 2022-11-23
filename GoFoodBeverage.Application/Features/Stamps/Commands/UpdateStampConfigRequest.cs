using AutoMapper;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stamps.Commands
{
    public class UpdateStampConfigRequest : IRequest<bool>
    {
        public EnumStampType StampType { get; set; }

        public bool IsShowLogo { get; set; }

        public bool IsShowTime { get; set; }

        public bool IsShowNumberOfItem { get; set; }

        public bool IsShowNote { get; set; }
    }

    public class UpdateStampConfigRequestHandler : IRequestHandler<UpdateStampConfigRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateStampConfigRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateStampConfigRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var stampConfig = await _unitOfWork.StampConfigs
                .GetStampConfigByStoreIdAsync(loggedUser.StoreId);

            if (stampConfig == null)
            {
                //Add New
                var stampConfigAddNew = CreateStampConfig(request, loggedUser.AccountId.Value, loggedUser.StoreId.Value);

                await _unitOfWork.StampConfigs.AddAsync(stampConfigAddNew);
            }
            else
            {
                //Update
                var stampConfigUpdate = UpdateStampConfig(stampConfig, request, loggedUser.AccountId.Value);

                await _unitOfWork.StampConfigs.UpdateAsync(stampConfigUpdate);
            }

            return true;
        }

        public static StampConfig CreateStampConfig(UpdateStampConfigRequest request, Guid accountId, Guid storeId)
        {
            var stampConfig = new StampConfig()
            {
                StoreId = storeId,
                StampType = request.StampType,
                IsShowLogo = request.IsShowLogo,
                IsShowTime = request.IsShowTime,
                IsShowNumberOfItem = request.IsShowNumberOfItem,
                IsShowNote = request.IsShowNote,
                CreatedUser = accountId
            };

            return stampConfig;
        }

        public static StampConfig UpdateStampConfig(StampConfig stampConfig, UpdateStampConfigRequest request, Guid accountId)
        {
            stampConfig.StampType = request.StampType;
            stampConfig.IsShowLogo = request.IsShowLogo;
            stampConfig.IsShowTime = request.IsShowTime;
            stampConfig.IsShowNumberOfItem = request.IsShowNumberOfItem;
            stampConfig.IsShowNote = request.IsShowNote;
            stampConfig.LastSavedUser = accountId;

            return stampConfig;
        }
    }
}

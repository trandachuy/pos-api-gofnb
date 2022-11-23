using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Barcodes.Commands
{
    public class UpdateBarcodeConfigRequest : IRequest<bool>
    {
        public EnumStampType StampType { get; set; }

        public EnumBarcodeType BarcodeType { get; set; }

        public bool IsShowName { get; set; }

        public bool IsShowPrice { get; set; }

        public bool IsShowCode { get; set; }
    }

    public class UpdateBarcodeConfigRequestHandler : IRequestHandler<UpdateBarcodeConfigRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdateBarcodeConfigRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdateBarcodeConfigRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var barcodeConfig = await _unitOfWork.BarcodeConfigs
                .GetBarcodeConfigByStoreIdAsync(loggedUser.StoreId);

            if (barcodeConfig == null)
            {
                var barcodeConfigAddNew = CreateBarcodeConfig(request, loggedUser.AccountId.Value, loggedUser.StoreId.Value);

                await _unitOfWork.BarcodeConfigs.AddAsync(barcodeConfigAddNew);
            }
            else
            {
                var barcodeConfigUpdate = UpdateBarcodeConfig(barcodeConfig, request, loggedUser.AccountId.Value);

                await _unitOfWork.BarcodeConfigs.UpdateAsync(barcodeConfigUpdate);
            }

            return true;
        }

        public static BarcodeConfig CreateBarcodeConfig(UpdateBarcodeConfigRequest request, Guid accountId, Guid storeId)
        {
            var barcodeConfig = new BarcodeConfig()
            {
                StoreId = storeId,
                StampType = request.StampType,
                BarcodeType = request.BarcodeType,
                IsShowName = request.IsShowName,
                IsShowPrice = request.IsShowPrice,
                IsShowCode = request.IsShowCode,
                CreatedUser = accountId
            };

            return barcodeConfig;
        }

        public static BarcodeConfig UpdateBarcodeConfig(BarcodeConfig barcodeConfig, UpdateBarcodeConfigRequest request, Guid accountId)
        {
            barcodeConfig.StampType = request.StampType;
            barcodeConfig.BarcodeType = request.BarcodeType;
            barcodeConfig.IsShowName = request.IsShowName;
            barcodeConfig.IsShowPrice = request.IsShowPrice;
            barcodeConfig.IsShowCode = request.IsShowCode;
            barcodeConfig.LastSavedUser = accountId;

            return barcodeConfig;
        }
    }
}

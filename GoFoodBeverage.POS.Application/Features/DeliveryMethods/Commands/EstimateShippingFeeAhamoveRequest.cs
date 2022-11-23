using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.DeliveryMethods.Commands
{
    public class EstimateShippingFeeAhamoveRequest : IRequest<EstimateShippingFeeAhamoveResponse>
    {
        public EstimateOrderAhamoveRequestModel EstimateOrderAhamoveRequest { get; set; }
    }

    public class EstimateShippingFeeAhamoveResponse
    {
        public EstimatedOrderAhamoveFeeResponseModel EstimatedOrderFeeResponse { get; set; }
    }

    public class EstimateShippingFeeAhamoveRequestHandler : IRequestHandler<EstimateShippingFeeAhamoveRequest, EstimateShippingFeeAhamoveResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAhamoveService _ahaMoveService;
        private readonly IUserProvider _userProvider;

        public EstimateShippingFeeAhamoveRequestHandler(
            IUnitOfWork unitOfWork,
            IAhamoveService ahaMoveService,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _ahaMoveService = ahaMoveService;
            _userProvider = userProvider;
        }

        public async Task<EstimateShippingFeeAhamoveResponse> Handle(EstimateShippingFeeAhamoveRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var ahamoveConfig = await _unitOfWork.DeliveryConfigs
                .Find(dc => dc.StoreId == loggedUser.StoreId && dc.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove)
                .FirstOrDefaultAsync(cancellationToken);

            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahamoveConfig.PhoneNumber,
                Name = ahamoveConfig.Name,
                ApiKey = ahamoveConfig.ApiKey,
                Address = ahamoveConfig.Address,
            };

            var infoToken = await _ahaMoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);
            var feeInfo = new EstimatedOrderAhamoveFeeResponseModel();

            if (infoToken != null && infoToken.Token != null)
            {
                feeInfo = await _ahaMoveService.EstimateOrderFee(infoToken.Token, request.EstimateOrderAhamoveRequest);
            }

            var response = new EstimateShippingFeeAhamoveResponse()
            {
                EstimatedOrderFeeResponse = feeInfo
            };

            return response;
        }
    }
}

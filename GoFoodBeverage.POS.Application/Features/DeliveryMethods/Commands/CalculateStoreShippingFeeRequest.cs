using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.DeliveryMethod;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.DeliveryMethods.Commands
{
    public class CalculateStoreShippingFeeRequest : IRequest<CalculateStoreShippingFeeResponse>
    {
        public double Distance { get; set; }
    }

    public class CalculateStoreShippingFeeResponse
    {
        public decimal ShippingFee { get; set; }
    }

    public class CalculateStoreShippingFeeRequestHandler : IRequestHandler<CalculateStoreShippingFeeRequest, CalculateStoreShippingFeeResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CalculateStoreShippingFeeRequestHandler(
           IUserProvider userProvider,
           IUnitOfWork unitOfWork,
           IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CalculateStoreShippingFeeResponse> Handle(CalculateStoreShippingFeeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var deliveryConfig = await _unitOfWork.DeliveryConfigs.GetAll()
                .Where(x => x.DeliveryMethodEnumId == EnumDeliveryMethod.SelfDelivery && x.StoreId == loggedUser.StoreId)
                .Include(x => x.DeliveryConfigPricings)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            decimal shippingFee = 0;
            var deliveryConfigModel = new DeliveryConfigModel();

            if (deliveryConfig != null)
            {
                deliveryConfigModel = _mapper.Map<DeliveryConfigModel>(deliveryConfig);

                ///Fixed fee
                if (deliveryConfigModel.IsFixedFee)
                {
                    shippingFee = deliveryConfigModel.FeeValue;
                }
                ///Distance fee
                else
                {
                    var deliveryConfigPricings = deliveryConfigModel.DeliveryConfigPricings;
                    var feeValue = deliveryConfigPricings.Where(x => x.FromDistanceMeter <= request.Distance && x.ToDistanceMeter >= request.Distance).Select(x => x.FeeValue).FirstOrDefault();
                    
                    if (feeValue > 0)
                    {
                        shippingFee = feeValue;
                    }
                    ///If the distance not match any delivery price config, get the highest fee 
                    else
                    {
                        shippingFee = deliveryConfigPricings.Max(x => x.FeeValue);
                    }
                }
            }

            return new CalculateStoreShippingFeeResponse
            {
                ShippingFee = shippingFee,
            };
        }
    }
}

using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Payments.Queries
{
    public class GetPaymentConfigurationByStoreIdRequest : IRequest<GetPaymentConfigurationByStoreIdResponse>
    {
        public Guid? StoreId { get; set; }
    }

    public class GetPaymentConfigurationByStoreIdResponse
    {
        public List<PaymentMethodByStoreModel> PaymentMethods { get; set; }
    }

    public class GetPaymentConfigurationByStoreIdRequestHandler : IRequestHandler<GetPaymentConfigurationByStoreIdRequest, GetPaymentConfigurationByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPaymentConfigurationByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetPaymentConfigurationByStoreIdResponse> Handle(GetPaymentConfigurationByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var paymentMethods = await _unitOfWork.PaymentMethods
               .GetAll()
               .Include(pm => pm.PaymentConfigs.Where(pmc => pmc.StoreId == request.StoreId && pmc.IsActivated == true))
               .AsNoTracking()
               .Where(pm => pm.PaymentConfigs.Count(pc => pc.StoreId == request.StoreId && pc.IsActivated == true) > 0)
               .ToListAsync(cancellationToken: cancellationToken);

            var paymentMethodModels = _mapper.Map<List<PaymentMethodByStoreModel>>(paymentMethods);

            var response = new GetPaymentConfigurationByStoreIdResponse()
            {
                PaymentMethods = paymentMethodModels
            };

            return response;
        }
    }
}

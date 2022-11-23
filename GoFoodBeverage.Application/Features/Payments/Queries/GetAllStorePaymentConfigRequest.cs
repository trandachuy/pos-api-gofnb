using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
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
    public class GetAllStorePaymentConfigRequest : IRequest<GetAllStorePaymentConfigResponse>
    {
    }

    public class GetAllStorePaymentConfigResponse
    {
        public IEnumerable<PaymentMethodModel> PaymentMethods { get; set; }
    }

    public class GetAllStorePaymentConfigRequestHandler : IRequestHandler<GetAllStorePaymentConfigRequest, GetAllStorePaymentConfigResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllStorePaymentConfigRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetAllStorePaymentConfigResponse> Handle(GetAllStorePaymentConfigRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var paymentMethods = await _unitOfWork.PaymentMethods
               .GetAll()
               .AsNoTracking()
               .Include(pm => pm.PaymentConfigs.Where(pm => pm.StoreId == loggedUser.StoreId))
               .ToListAsync(cancellationToken: cancellationToken);

            var listPaymentConfigAdd = new List<PaymentConfig>();
            var cashPaymentMethod = paymentMethods.FirstOrDefault(x => x.EnumId == EnumPaymentMethod.Cash);
            if (cashPaymentMethod.PaymentConfigs.Count == 0)
            {
                var paymentConfig = new PaymentConfig()
                {
                    PaymentMethodId = cashPaymentMethod.Id,
                    StoreId = loggedUser.StoreId,
                    PaymentMethodEnumId = EnumPaymentMethod.Cash,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = true,
                    IsAuthenticated = true
                };
                listPaymentConfigAdd.Add(paymentConfig);
            }

            var vnpayMethod = paymentMethods.FirstOrDefault(x => x.EnumId == EnumPaymentMethod.VNPay);
            if (vnpayMethod.PaymentConfigs.Count == 0)
            {
                var paymentConfig = new PaymentConfig()
                {
                    PaymentMethodId = vnpayMethod.Id,
                    StoreId = loggedUser.StoreId,
                    PaymentMethodEnumId = EnumPaymentMethod.VNPay,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = false,
                    IsAuthenticated = false
                };
                listPaymentConfigAdd.Add(paymentConfig);
            }

            var visaMethod = paymentMethods.FirstOrDefault(x => x.EnumId == EnumPaymentMethod.CreditDebitCard);
            if (visaMethod.PaymentConfigs.Count == 0)
            {
                var paymentConfig = new PaymentConfig()
                {
                    PaymentMethodId = visaMethod.Id,
                    StoreId = loggedUser.StoreId,
                    PaymentMethodEnumId = EnumPaymentMethod.CreditDebitCard,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = false,
                    IsAuthenticated = false
                };
                listPaymentConfigAdd.Add(paymentConfig);
            }

            var momoMethod = paymentMethods.FirstOrDefault(x => x.EnumId == EnumPaymentMethod.MoMo);
            if (momoMethod.PaymentConfigs.Count == 0)
            {
                var paymentConfig = new PaymentConfig()
                {
                    PaymentMethodId = momoMethod.Id,
                    StoreId = loggedUser.StoreId,
                    PaymentMethodEnumId = EnumPaymentMethod.MoMo,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = false,
                    IsAuthenticated = false
                };

                listPaymentConfigAdd.Add(paymentConfig);
            }
            if (listPaymentConfigAdd.Count > 0)
            {
                await _unitOfWork.PaymentConfigs.AddRangeAsync(listPaymentConfigAdd);
            }

            var result = new List<PaymentMethod>();
            result.Add(cashPaymentMethod);
            result.Add(vnpayMethod);
            result.Add(visaMethod);
            result.Add(momoMethod);
            var paymentMethodModels = _mapper.Map<List<PaymentMethodModel>>(result);

            var response = new GetAllStorePaymentConfigResponse()
            {
                PaymentMethods = paymentMethodModels
            };

            return response;
        }
    }
}

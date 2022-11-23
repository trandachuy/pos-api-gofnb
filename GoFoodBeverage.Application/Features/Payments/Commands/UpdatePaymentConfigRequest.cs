using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.MoMo.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Payments.Commands
{
    public class UpdatePaymentConfigRequest : IRequest<bool>
    {
        public EnumPaymentMethod EnumId { get; set; }

        public Guid? PaymentMethodId { get; set; }

        public string PartnerCode { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string QRCode { get; set; }
    }

    public class UpdatePaymentConfigRequestHandler : IRequestHandler<UpdatePaymentConfigRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;
        private readonly IMoMoPaymentService _momoPaymentService;

        public UpdatePaymentConfigRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService,
            IMoMoPaymentService momoPaymentService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
            _momoPaymentService = momoPaymentService;
        }

        public async Task<bool> Handle(UpdatePaymentConfigRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

                switch (request.EnumId)
                {
                    case EnumPaymentMethod.MoMo:
                        await UpdateMoMoPaymentConfigAsync(request, loggedUser);
                        break;

                    case EnumPaymentMethod.CreditDebitCard:
                        await UpdateCreditDebitCardPaymentConfigAsync(request, loggedUser);
                        break;

                    case EnumPaymentMethod.VNPay:
                        await UpdateVNPayPaymentConfigAsync(request, loggedUser);
                        break;

                    default:
                        break;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task AddNewPaymentConfigAsync(UpdatePaymentConfigRequest request, LoggedUserModel loggedUser, bool isAuthenticated)
        {
            var paymentConfig = new PaymentConfig()
            {
                StoreId = loggedUser.StoreId,
                PaymentMethodId = request.PaymentMethodId,
                PaymentMethodEnumId = request.EnumId,
                PartnerCode = request.PartnerCode,
                AccessKey = request.AccessKey,
                SecretKey = request.SecretKey,
                QRCode = request.QRCode,
                CreatedUser = loggedUser.AccountId,
                LastSavedUser = loggedUser.AccountId,
                IsActivated = true,
                IsAuthenticated = isAuthenticated
            };

            _unitOfWork.PaymentConfigs.Add(paymentConfig);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateMoMoPaymentConfigAsync(UpdatePaymentConfigRequest request, LoggedUserModel loggedUser)
        {
            var paymentConfigExisted = await _unitOfWork.PaymentConfigs
                .Find(p => p.StoreId == loggedUser.StoreId && p.PaymentMethodEnumId == request.EnumId)
                .FirstOrDefaultAsync();
            var createGateway = await CreateGatewayAsync(request, loggedUser);
            var isAuthenticated = createGateway != null && createGateway.ResultCode == 0;

            if (paymentConfigExisted == null)
            {
                await AddNewPaymentConfigAsync(request, loggedUser, isAuthenticated);
                return;
            }

            paymentConfigExisted.PartnerCode = request.PartnerCode;
            paymentConfigExisted.SecretKey = request.SecretKey;
            paymentConfigExisted.AccessKey = request.AccessKey;
            paymentConfigExisted.IsActivated = true;
            paymentConfigExisted.LastSavedUser = loggedUser.AccountId;

            _unitOfWork.PaymentConfigs.Update(paymentConfigExisted);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateCreditDebitCardPaymentConfigAsync(UpdatePaymentConfigRequest request, LoggedUserModel loggedUser)
        {
            var paymentConfigExisted = await _unitOfWork.PaymentConfigs
                .Find(p => p.StoreId == loggedUser.StoreId && p.PaymentMethodEnumId == request.EnumId)
                .FirstOrDefaultAsync();

            if (paymentConfigExisted == null)
            {
                var paymentConfig = new PaymentConfig()
                {
                    StoreId = loggedUser.StoreId,
                    PaymentMethodId = request.PaymentMethodId,
                    PaymentMethodEnumId = request.EnumId,
                    PartnerCode = request.PartnerCode,
                    SecretKey = request.SecretKey,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = true,
                };

                _unitOfWork.PaymentConfigs.Add(paymentConfig);
            }
            else
            {
                paymentConfigExisted.PartnerCode = request.PartnerCode;
                paymentConfigExisted.SecretKey = request.SecretKey;
                paymentConfigExisted.IsActivated = true;
                paymentConfigExisted.LastSavedUser = loggedUser.AccountId;

                _unitOfWork.PaymentConfigs.Update(paymentConfigExisted);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateVNPayPaymentConfigAsync(UpdatePaymentConfigRequest request, LoggedUserModel loggedUser)
        {
            var paymentConfigExisted = await _unitOfWork.PaymentConfigs
                .Find(p => p.StoreId == loggedUser.StoreId && p.PaymentMethodEnumId == request.EnumId)
                .FirstOrDefaultAsync();

            if (paymentConfigExisted == null)
            {
                var paymentConfig = new PaymentConfig()
                {
                    StoreId = loggedUser.StoreId,
                    PaymentMethodId = request.PaymentMethodId,
                    PaymentMethodEnumId = request.EnumId,
                    PartnerCode = request.PartnerCode,
                    SecretKey = request.SecretKey,
                    CreatedUser = loggedUser.AccountId,
                    LastSavedUser = loggedUser.AccountId,
                    IsActivated = true,
                };

                _unitOfWork.PaymentConfigs.Add(paymentConfig);
            }
            else
            {
                paymentConfigExisted.PartnerCode = request.PartnerCode;
                paymentConfigExisted.SecretKey = request.SecretKey;
                paymentConfigExisted.IsActivated = true;
                paymentConfigExisted.LastSavedUser = loggedUser.AccountId;

                _unitOfWork.PaymentConfigs.Update(paymentConfigExisted);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<CreateGetwayResponseModel> CreateGatewayAsync(UpdatePaymentConfigRequest request, LoggedUserModel loggedUser)
        {
            var paymentConfig = new PartnerMoMoPaymentConfigModel()
            {
                AccessKey = request.AccessKey,
                PartnerCode = request.PartnerCode,
                SecretKey = request.SecretKey
            };

            var site = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";

            var nameEmail = loggedUser.Email.Split("+")[0];
            var emainDomain = loggedUser.Email.Split("@")[1];
            var requestGateway = new CreateGetwayRequestModel()
            {
                RequestId = Guid.NewGuid().ToString(),
                Amount = "0",
                ExtraData = "",
                OrderId = Guid.NewGuid().ToString(),
                OrderInfo = $"CreateGateway{request.PartnerCode}",
                PartnerClientId = $"{nameEmail}@{emainDomain}",
                RedirectUrl = site,
                IpnUrl = site
            };

            try
            {
                var result = await _momoPaymentService.CreateGatewayTestingAsync(paymentConfig, requestGateway);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("payment.authenticationFail");
            }
        }
    }
}

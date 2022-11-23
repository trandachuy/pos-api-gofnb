using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.MoMo.Enums;
using GoFoodBeverage.Payment.MoMo.Model;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Payment.VNPay.Enums;
using GoFoodBeverage.Payment.VNPay.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services
{
    [AutoService(typeof(IPaymentService), Lifetime = ServiceLifetime.Scoped)]
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMoMoPaymentService _momoPaymentService;
        private readonly IVNPayService _vnPayService;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTimeService;

        public PaymentService(IUnitOfWork unitOfWork,
            IMoMoPaymentService momoPaymentService,
            IVNPayService vnPayService,
            IUserProvider userProvider,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _momoPaymentService = momoPaymentService;
            _vnPayService = vnPayService;
            _userProvider = userProvider;
            _dateTimeService = dateTimeService;
        }

        public async Task<bool> PaymentRefundAsync(PaymentRefundRequestModel request)
        {
            var loggedUser = await _userProvider.ProvideAsync();

            var orderTransaction = await _unitOfWork.OrderPaymentTransactions
                .Find(item => item.OrderId == request.OrderId)
                .Select(item => new
                {
                    item.Id,
                    item.OrderId,
                    item.Amount,
                    item.PaymentMethodId,
                    item.TransId,
                    item.OrderInfo,
                })
                .FirstOrDefaultAsync();

            ThrowError.BadRequestAgainstNull(orderTransaction, "Cannot find transaction");

            if (orderTransaction != null)
                switch (request.PaymentMethod.ToUpper())
                {
                    case RefundPaymentMethodConstants.MOMO:
                        /// Handle refund from momo
                        var paymentConfigMomo = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.MoMo);

                        var config = new PartnerMoMoPaymentConfigModel()
                        {
                            PartnerCode = paymentConfigMomo.PartnerCode,
                            AccessKey = paymentConfigMomo.AccessKey,
                            SecretKey = paymentConfigMomo.SecretKey,
                        };

                        // Add a new payment transaction
                        var orderMomoPaymentTransaction = new OrderPaymentTransaction()
                        {
                            IsSuccess = false,
                            OrderId = request.OrderId,
                            OrderInfo = $"Refund - {orderTransaction.OrderId}",
                            CreatedUser = loggedUser.AccountId.Value,
                            PaymentMethodId = EnumPaymentMethod.MoMo,
                            TransactionType = EnumTransactionType.Refund,
                        };

                        var refundMomoRequest = new CreateRefundRequest()
                        {
                            OrderId = orderMomoPaymentTransaction.Id.ToString(),
                            RequestId = Guid.NewGuid().ToString(),
                            Amount = Convert.ToInt64(orderTransaction.Amount),
                            TransId = orderTransaction.TransId.Value,
                            Lang = "vi",
                            Description = "Refund momo",
                        };
                        var response = await _momoPaymentService.CreateRefundAsync(config, refundMomoRequest);

                        if (response != null && response.ResultCode.ToString() == MomoResponseCode.Success)
                        {
                            orderMomoPaymentTransaction.IsSuccess = true;
                            orderMomoPaymentTransaction.TransId = response?.TransId;
                            orderMomoPaymentTransaction.Amount = response.Amount;
                            orderMomoPaymentTransaction.ExtraData = response.Message;
                            orderMomoPaymentTransaction.ResponseData = response.Message;
                        }

                        //Save payment transaction
                        await _unitOfWork.OrderPaymentTransactions.AddAsync(orderMomoPaymentTransaction);

                        break;

                    case RefundPaymentMethodConstants.VNPAY_WALLER:
                    case RefundPaymentMethodConstants.VISA_MASTER_CARD:
                        /// Get the store's payment configuration.
                        var vnpayPaymentStoreConfig = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(request.StoreId, EnumPaymentMethod.VNPay);
                        var vnPayConfig = new VNPayConfigModel()
                        {
                            TerminalId = vnpayPaymentStoreConfig.PartnerCode,
                            SecretKey = vnpayPaymentStoreConfig.SecretKey,
                        };

                        // Add a new payment transaction.
                        var orderPaymentTransaction = new OrderPaymentTransaction()
                        {
                            IsSuccess = false,
                            OrderInfo = $"Refund - {orderTransaction.OrderId}",
                            Amount = orderTransaction.Amount,
                            OrderId = request.OrderId,
                            //TransId = orderTransaction.TransId, // TODO: Transaction id
                            //ExtraData = orderInfo.ExtraData, // TODO: Extra data
                            CreatedUser = loggedUser.AccountId.Value,
                            PaymentMethodId = EnumPaymentMethod.VNPay,
                            TransactionType = EnumTransactionType.Refund,
                        };

                        var refundRequest = new VNPayRefundRequestModel()
                        {
                            RequestId = Guid.NewGuid(),
                            TransactionId = orderTransaction.Id,
                            TransactionType = EnumVNPayTransactionType.FullRefund.GetCode(),
                            Amount = orderTransaction.Amount,
                            OrderInfo = orderTransaction.OrderInfo,
                            //TransDate = orderTransaction.RequestCreatedDate, /// TODO: update payment transaction table
                            CreateBy = $"{loggedUser.AccountId}",
                            CreateDate = _dateTimeService.NowUtc,
                        };

                        var vnpayRefundResponse = await _vnPayService.RefundAsync(vnPayConfig, refundRequest);
                        if (vnpayRefundResponse != null && vnpayRefundResponse.vnp_ResponseCode == VNPayResponseCode.Success)
                        {
                            orderPaymentTransaction.IsSuccess = true;
                        }

                        // Save payment transaction.
                        await _unitOfWork.OrderPaymentTransactions.AddAsync(orderPaymentTransaction);

                        break;
                }

            return true;
        }
    }
}

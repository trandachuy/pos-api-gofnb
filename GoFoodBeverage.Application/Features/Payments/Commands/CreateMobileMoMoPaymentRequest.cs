using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.MoMo.Enums;
using GoFoodBeverage.Payment.MoMo.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Payments.Commands
{
    public class CreateMobileMoMoPaymentRequest : IRequest<CreateGetwayResponseModel>
    {
        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }

        public string Amount { get; set; }

        public string OrderInfo { get; set; }

        public Guid OrderId { get; set; }
    }

    public class CreateMobileMoMoPaymentRequestHandler : IRequestHandler<CreateMobileMoMoPaymentRequest, CreateGetwayResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMoMoPaymentService _momoPaymentService;
        private readonly IUserProvider _userProvider;

        public CreateMobileMoMoPaymentRequestHandler(IUnitOfWork unitOfWork, IMoMoPaymentService momoPaymentService, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _momoPaymentService = momoPaymentService;
            _userProvider = userProvider;
        }

        public async Task<CreateGetwayResponseModel> Handle(CreateMobileMoMoPaymentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = _userProvider.GetLoggedCustomer();
            var paymentConfigMomo = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(request.StoreId, EnumPaymentMethod.MoMo);

            var storeInfo = _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(request.BranchId)
                .Include(x => x.Store)
                .Select(x => new
                {
                    StoreName = x.Store.Title,
                    BranchName = x.Name,
                }).FirstOrDefault();

            var orderInfo = $"Payment for order {request?.OrderInfo} at {storeInfo.StoreName} - {storeInfo.BranchName}";

            var config = new PartnerMoMoPaymentConfigModel()
            {
                PartnerCode = paymentConfigMomo.PartnerCode,
                AccessKey = paymentConfigMomo.AccessKey,
                SecretKey = paymentConfigMomo.SecretKey,
            };

            var extractDataJson = new JObject
            {
                { "orderId", request.OrderId.ToString() },
                { "amount", request.Amount },
            };
            var extractData = Base64Encode(extractDataJson.ToString());

            //Handle Create OrderPaymentTransaction
            var orderPaymentTransaction = new OrderPaymentTransaction()
            {
                OrderId = request.OrderId,
                PaymentMethodId = (int)EnumPaymentMethod.MoMo,
                TransId = 0,
                OrderInfo = orderInfo,
                Amount = Decimal.Parse(request.Amount),
                ExtraData = extractData,
                IsSuccess = false,
                CreatedUser = loggedUser.AccountId.Value
            };
            await _unitOfWork.OrderPaymentTransactions.AddAsync(orderPaymentTransaction);

            //Handle Payment Request
            var paymentRequest = new CreateGetwayRequestModel()
            {
                RequestId = orderPaymentTransaction.Id.ToString(),
                Amount = request.Amount,
                OrderId = orderPaymentTransaction.OrderId.ToString(),
                OrderInfo = orderInfo,
                RedirectUrl = AppOrderUrlConstants.ORDER_DETAIL,
                IpnUrl = "https://momo.vn",
                PartnerClientId = loggedUser.Email,
                ExtraData = extractData
            };

            /// Thanh Toán Thông Thường
            /// https://developers.momo.vn/v3/vi/docs/payment/api/wallet/onetime
            try
            {
                var response = await _momoPaymentService.CreateGatewayAsync(config, paymentRequest, RequestTypes.CaptureWallet);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}

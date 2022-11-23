using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Payment.VNPay;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Payment.VNPay.Model;

namespace GoFoodBeverage.POS.Application.Features.Payments.Commands
{
    public class CreateVNPayPaymentRequest : IRequest<CreateVNPayPaymentResponse>
    {
        public string VNPayBankCode { get; set; }

        public int Amount { get; set; }

        public string OrderInfo { get; set; }

        public string RedirectUrl { get; set; }

        public Guid OrderId { get; set; }
    }

    public class CreateVNPayPaymentResponse
    {
        public string PaymentUrl { get; set; }

        public VNPayOrderInfoModel OrderInfo { get; set; }
    }

    public class CreateVNPayPaymentRequestHandler : IRequestHandler<CreateVNPayPaymentRequest, CreateVNPayPaymentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IVNPayService _vnPayService;

        public CreateVNPayPaymentRequestHandler(IUnitOfWork unitOfWork, IUserProvider userProvider, IVNPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _vnPayService = vnPayService;
        }

        public async Task<CreateVNPayPaymentResponse> Handle(CreateVNPayPaymentRequest request, CancellationToken cancellationToken)
        {
            var locale = "vn";
            var returnUrl = request.RedirectUrl;

            // Get the current user information from request.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            //Get store branch information 
            var storeInfo = _unitOfWork.StoreBranches.Find(x => x.StoreId == loggedUser.StoreId && x.Id == loggedUser.BranchId.Value && !x.IsDeleted)
                .Include(x => x.Store)
                .Select(x => new
                {
                    StoreName = x.Store.Title,
                    BranchName = x.Name,
                }).FirstOrDefault();

            var title = $"Payment for order {request.OrderInfo} at {storeInfo.StoreName} - {storeInfo.BranchName}";

            // Get the store's payment configuration.
            var paymentConfigForVnPay = await _unitOfWork.
                PaymentConfigs.
                GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.VNPay);

            // The token is used to access the payment provider VNPay.
            var config = new VNPayConfigModel();
            config.TerminalId = paymentConfigForVnPay.PartnerCode;
            config.SecretKey = paymentConfigForVnPay.SecretKey;

            var orderInfo = new VNPayOrderInfoModel()
            {
                Title = title,
                Amount = request.Amount,
                CreatedDate = DateTime.UtcNow,
                Status = "0",
                BankCode = request.VNPayBankCode,
                PayStatus = "",
                CurrencyCode = "VND"
            };

            // Call the VNPay's service.
            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(config, orderInfo, locale, returnUrl);

            orderInfo.OrderDesc = $"Desc: {orderInfo.OrderId}";
            orderInfo.PaymentTranId = orderInfo.OrderId;

            // Add a new payment transaction.
            var orderPaymentTransaction = new OrderPaymentTransaction()
            {
                IsSuccess = false,
                OrderInfo = title,
                Amount = request.Amount,
                OrderId = request.OrderId,
                TransId = orderInfo.OrderId,
                ExtraData = orderInfo.OrderId.ToString(),
                CreatedUser = loggedUser.AccountId.Value,
                PaymentMethodId = EnumPaymentMethod.VNPay,
            };

            // Save payment transaction.
            await _unitOfWork.OrderPaymentTransactions.AddAsync(orderPaymentTransaction);

            // Return data to the client.
            var response = new CreateVNPayPaymentResponse()
            {
                PaymentUrl = paymentUrl,
                OrderInfo = orderInfo
            };

            return response;
        }
    }
}

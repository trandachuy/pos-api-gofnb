using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Payment.VNPay.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Commands
{
    public class CreateVNPayPaymentRequest : IRequest<CreateVNPayPaymentResponse>
    {
        private long _Amount;

        public long Amount
        {
            get { return _Amount; }
            set
            {
                ThrowError.Against(value <= 0, "Amount more than by 0");
                _Amount = value;
            }
        }

        public Guid PackageId { get; set; }

        public int PackageDurationByMonth { get; set; }

        public EnumPackagePaymentMethod PaymentMethod { get; set; }

        public string RedirectUrl { get; set; }
    }

    public class CreateVNPayPaymentResponse
    {
        public string PaymentUrl { get; set; }

        public int Code { get; set; }

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
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var store = await _unitOfWork.Stores
                .Find(x => x.Id == loggedUser.StoreId)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync();
          
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            ThrowError.Against(string.IsNullOrEmpty(store.Currency.Code), "Cannot find currencyCode information");
            
            var locale = "vn";
            var returnUrl = request.RedirectUrl;
            var package = _unitOfWork.Packages.Find(x => x.Id == request.PackageId).FirstOrDefault();
            var originalPrice = request.Amount;
            var packageOrderModelAdd = new OrderPackage
            {
                PackageId = request.PackageId,
                PackageDurationByMonth = request.PackageDurationByMonth,
                TotalAmount = (int)originalPrice,
                PackageOderPaymentStatus = EnumOrderPaymentStatus.Unpaid,
                PackagePaymentMethod = request.PaymentMethod,
                Status = EnumOrderPackageStatus.PENDING.GetName(),
                StoreId = store.Id,
                Email = loggedUser.Email,
                ShopPhoneNumber = loggedUser.PhoneNumber,
                SellerName = loggedUser.FullName,
                ShopName = store.Title,
                OrderPackageType = EnumOrderPackageType.StoreActivate,
                IsActivated = false
            };

            var packageOrderNew = await _unitOfWork.OrderPackages.AddAsync(packageOrderModelAdd);
            var accountTransfer = _unitOfWork.AccountTransfers.GetAll().FirstOrDefault();

            // Get the store's payment configuration.
            // var paymentConfigForVnPay = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.VNPay);
            // Get the store's payment of mediastep
            var paymentConfigForVnPay = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(PaymentConfigConstants.MediaStepStoreId, EnumPaymentMethod.VNPay);
            // The token is used to access the payment provider VNPay.
            var vnpayConfig = new VNPayConfigModel
            {
                TerminalId = paymentConfigForVnPay.PartnerCode,
                SecretKey = paymentConfigForVnPay.SecretKey
            };
            var vnPayOrderId = DateTime.Now.Ticks;
            var orderInfo = new VNPayOrderInfoModel()
            {
                OrderId = vnPayOrderId,
                Title = $"{package.Name}_{originalPrice}",
                Amount = originalPrice,
                OrderDesc = $"Desc: {vnPayOrderId}",
                CreatedDate = DateTime.UtcNow,
                Status = ((int)EnumVNPayStatus.Pending).ToString(),
                PaymentTranId = vnPayOrderId,
                BankCode = request.PaymentMethod.GetBankCode(),
                PayStatus = "",
                CurrencyCode = store.Currency.Code
            };

            orderInfo.VnPayCreateDate = orderInfo.CreatedDate.VnPayFormatDate();

            // Call the VNPay's service.
            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(vnpayConfig, orderInfo, locale, returnUrl);

            // Return data to the client.
            var response = new CreateVNPayPaymentResponse()
            {
                PaymentUrl = paymentUrl,
                OrderInfo = orderInfo,
                Code = packageOrderNew.Code
            };

            return response;
        }
    }
}

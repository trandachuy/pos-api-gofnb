using GoFoodBeverage.Application.Features.Stores.Queries;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Payment.VNPay.Enums;
using GoFoodBeverage.Payment.VNPay.Model;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Commands
{
    public class UpdateVNPayPaymentRequest : IRequest<VnPayUpdateOrderByQrCodeResponse>
    {
        public int Code { get; set; }

        public string VnPayCreateDate { get; set; }

        public string OrderInfo { get; set; }

        public string TxnRef { get; set; }
    }

    public class VnPayUpdateOrderByQrCodeResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string PackageName { get; set; }

        public VnPayModel VnPayQuery { get; set; }

        public class VnPayModel
        {
            public VnPayModel(VNPayQueryPaymentStatusResponse vnpayData)
            {
                VnPayId = vnpayData.OrderId;

                ResponseCode = vnpayData.ResponseCode;
            }

            public string VnPayId { get; set; }

            public string ResponseCode { get; set; }
        }
    }

    public class RequestHandler : IRequestHandler<UpdateVNPayPaymentRequest, VnPayUpdateOrderByQrCodeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVNPayService _vnPayService;
        private readonly IUserProvider _userProvider;
        private readonly IMediator _mediator;

        public RequestHandler(IUnitOfWork unitOfWork, IVNPayService vnPayService, IUserProvider userProvider, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
            _userProvider = userProvider;
            _mediator = mediator;
        }

        /// <summary>
        /// This method is used to handle the current request.
        /// </summary>
        /// <param name="request">The HTTP data</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<VnPayUpdateOrderByQrCodeResponse> Handle(UpdateVNPayPaymentRequest request, CancellationToken cancellationToken)
        {
            // Get the current user information from request.
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            string newOrder = string.Empty;
            string oldOrder = string.Empty;

            // Get the store's payment configuration.
            var paymentConfigForVnPay = await _unitOfWork.
                PaymentConfigs.
                GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.VNPay);

            // The token is used to access the payment provider VNPay.
            var config = new VNPayConfigModel();
            config.TerminalId = paymentConfigForVnPay.PartnerCode;
            config.SecretKey = paymentConfigForVnPay.SecretKey;

            // Create a new query to check the real order.
            var resultFromVnPay = await _vnPayService.QueryAsync(
                config,
                request.TxnRef,
                request.OrderInfo,
                request.VnPayCreateDate,
                request.VnPayCreateDate
            );

            // Data to response to the client.
            var dataToResponse = new VnPayUpdateOrderByQrCodeResponse();

            // Handle data by the response code.
            switch (resultFromVnPay.ResponseCode)
            {
                case VNPayResponseCode.Success:
                    var packageOrder = _unitOfWork.OrderPackages.Find(x => x.Code == request.Code).FirstOrDefault();
                    if (packageOrder != null)
                    {
                        packageOrder.PackageOderPaymentStatus = EnumOrderPaymentStatus.Paid;
                        await _unitOfWork.OrderPackages.UpdateAsync(packageOrder);
                        var package = _unitOfWork.Packages.Find(x => x.Id == packageOrder.PackageId).FirstOrDefault();
                        dataToResponse.PackageName = package.Name;
                        dataToResponse.IsSuccess = true;
                        dataToResponse.Message = "package.payment.paymentSuccessful";
                        //Activate Account Store
                        var activateAccountStore = new ActivateAccountStoreRequest(){};
                        await _mediator.Send(activateAccountStore, cancellationToken);
                    }
                    break;
                case VNPayResponseCode.Other:
                    dataToResponse.Message = "package.payment.paymentUnsuccessful";
                    dataToResponse.VnPayQuery = new(resultFromVnPay);
                    break;
            }

            return dataToResponse;
        }
    }
}

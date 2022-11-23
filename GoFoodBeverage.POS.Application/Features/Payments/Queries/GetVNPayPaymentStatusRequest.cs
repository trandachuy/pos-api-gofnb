using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Payment.VNPay.Model;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Payments.Queries
{
    public class GetVNPayPaymentStatusRequest : IRequest<VNPayQueryPaymentStatusResponse>
    {
        public string OrderId { get; set; }

        public string Title { get; set; }

        public string CreateDate { get; set; }
    }

    public class GetVNPayPaymentStatusRequestHandler : IRequestHandler<GetVNPayPaymentStatusRequest, VNPayQueryPaymentStatusResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVNPayService _vnPayService;

        public GetVNPayPaymentStatusRequestHandler(IUnitOfWork unitOfWork, IVNPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
        }

        public async Task<VNPayQueryPaymentStatusResponse> Handle(GetVNPayPaymentStatusRequest request, CancellationToken cancellationToken)
        {
            var config = new VNPayConfigModel()
            {
                TerminalId = "XAPFAQUZ",
                SecretKey = "SNNGBOYXJTBIUCYUSVWJAPMJIRCULHMT"
            };

            var response = await _vnPayService.QueryAsync(config, request.OrderId, request.Title, request.CreateDate, request.CreateDate);

            return response;
        }
    }
}

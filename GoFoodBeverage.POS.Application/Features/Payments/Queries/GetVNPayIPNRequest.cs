using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Payment.VNPay.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Payments.Queries
{
    public class GetVNPayIPNRequest : IRequest<VNPayIPNResponse>
    {
        [BindProperty(Name = "vnp_Amount")]
        public string Amount { get; set; }

        [BindProperty(Name = "vnp_BankCode")]
        public string BankCode { get; set; }

        [BindProperty(Name = "vnp_OrderInfo")]
        public string OrderInfo { get; set; }

        [BindProperty(Name = "vnp_PayDate")]
        public string PayDate { get; set; }

        [BindProperty(Name = "vnp_ResponseCode")]
        public string ResponseCode { get; set; }

        [BindProperty(Name = "vnp_SecureHash")]
        public string SecureHash { get; set; }

        [BindProperty(Name = "vnp_TmnCode")]
        public string TerminalId { get; set; }

        [BindProperty(Name = "vnp_TransactionNo")]
        public string TransactionNo { get; set; }

        [BindProperty(Name = "vnp_TransactionStatus")]
        public string TransactionStatus { get; set; }

        [BindProperty(Name = "vnp_TxnRef")]
        public string OrderId { get; set; }
    }

    public class VNPayIPNResponse
    {
        public string Message { get; set; }

        public string RspCode { get; set; }
    }

    public class GetVNPayIPNRequestHandler : IRequestHandler<GetVNPayIPNRequest, VNPayIPNResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVNPayIPNRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VNPayIPNResponse> Handle(GetVNPayIPNRequest request, CancellationToken cancellationToken)
        {
            var response = new VNPayIPNResponse();

            switch (request.ResponseCode)
            {
                /// Test case: Giao dịch thành công
                case VNPayResponseCode.Success:
                    response.Message = "Confirm Success";
                    response.RspCode = request.ResponseCode;
                    break;

                /// Test case: Giao dịch không thành công
                case VNPayResponseCode.Other:
                    response.Message = "Confirm Success";
                    response.RspCode = "00";
                    break;


                    /// Implement another test case
            }

            return await Task.FromResult(response);
        }
    }
}

using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Payment;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Payments.Commands
{
    public class CreatePaymentRefundRequest : IRequest<bool>
    {
        public Guid StoreId { get; set; }

        public Guid OrderId { get; set; }

        public string PaymentMethod { get; set; }
    }

    public class CreatePaymentRefundRequestHandler : IRequestHandler<CreatePaymentRefundRequest, bool>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentRefundRequestHandler(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<bool> Handle(CreatePaymentRefundRequest request, CancellationToken cancellationToken)
        {
            var refundSuccess = await _paymentService.PaymentRefundAsync(new PaymentRefundRequestModel()
            {
                OrderId = request.OrderId,
                PaymentMethod = request.PaymentMethod,
                StoreId = request.StoreId
            });

            return refundSuccess;
        }
    }
}

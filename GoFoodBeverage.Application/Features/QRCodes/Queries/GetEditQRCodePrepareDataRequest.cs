using GoFoodBeverage.Models.QRCode;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Queries
{
    public class GetEditQRCodePrepareDataRequest : IRequest<GetEditQRCodePrepareDataResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetEditQRCodePrepareDataResponse
    {
        public QRCodeDetailDto QrCodeEditData { get; set; }

        public GetCreateQRCodePrepareDataResponse QrCodePrepareData { get; set; }
    }

    public class GetEditQRCodePrepareDataRequestHandler : IRequestHandler<GetEditQRCodePrepareDataRequest, GetEditQRCodePrepareDataResponse>
    {
        private readonly IMediator _mediator;

        public GetEditQRCodePrepareDataRequestHandler(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<GetEditQRCodePrepareDataResponse> Handle(GetEditQRCodePrepareDataRequest request, CancellationToken cancellationToken)
        {
            var qrCodeData = await _mediator.Send(new GetQRCodeDetailByIdRequest() { Id = request.Id }, cancellationToken);
            var qrCodePrepareData = await _mediator.Send(new GetCreateQRCodePrepareDataRequest(), cancellationToken);

            var response = new GetEditQRCodePrepareDataResponse()
            {
                QrCodeEditData = qrCodeData.QrCodeDetail,
                QrCodePrepareData = qrCodePrepareData,
            };

            return response;
        }
    }
}

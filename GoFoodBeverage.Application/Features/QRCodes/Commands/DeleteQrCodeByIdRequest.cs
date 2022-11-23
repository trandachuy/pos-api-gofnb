using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Commands
{
    public class DeleteQrCodeByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteQrCodeRequestHandler : IRequestHandler<DeleteQrCodeByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteQrCodeRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteQrCodeByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var qrCode = await _unitOfWork.QRCodes.GetQRCodeByIdAsync(request.Id, loggedUser.StoreId);
            ThrowError.Against(qrCode == null, "QrCode is not found");

            await _unitOfWork.QRCodes.RemoveAsync(qrCode);

            return true;
        }
    }
}

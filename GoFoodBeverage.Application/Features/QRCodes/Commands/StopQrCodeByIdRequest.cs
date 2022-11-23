using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Commands
{
    public class StopQrCodeByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class StopQrCodeByIdRequestHandler : IRequestHandler<StopQrCodeByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public StopQrCodeByIdRequestHandler(
            IUserProvider userProvider,
           IUnitOfWork unitOfWork,
           MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<bool> Handle(StopQrCodeByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var qrCode = await _unitOfWork.QRCodes.GetQRCodeByIdAsync(request.Id, loggedUser.StoreId);
            ThrowError.Against(qrCode == null, "Cannot find QrCode information");

            var modifiedQrCode = UpdateQrCode(qrCode, loggedUser.AccountId.Value);
            await _unitOfWork.QRCodes.UpdateAsync(modifiedQrCode);

            return true;
        }

        public static QRCode UpdateQrCode(QRCode qrCode, Guid accountId)
        {
            qrCode.IsStopped = true;
            qrCode.LastSavedUser = accountId;

            return qrCode;
        }
    }
}

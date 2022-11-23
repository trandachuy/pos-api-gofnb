using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using GoFoodBeverage.Models.QRCode;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Queries
{
    public class GetQRCodeDetailByIdRequest : IRequest<GetQRCodeDetailByIdRequestResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetQRCodeDetailByIdRequestResponse
    {
        public QRCodeDetailDto QrCodeDetail { get; set; }
    }

    public class GetQRCodeDetailByIdRequestHandler : IRequestHandler<GetQRCodeDetailByIdRequest, GetQRCodeDetailByIdRequestResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTime;
        private readonly IQrCodeService _qrCodeService;

        public GetQRCodeDetailByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IDateTimeService dateTime,
            IQrCodeService qrCodeService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTime = dateTime;
            _qrCodeService = qrCodeService;
        }

        public async Task<GetQRCodeDetailByIdRequestResponse> Handle(GetQRCodeDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var qrCodeData = await _unitOfWork.QRCodes.GetQRCodeByIdAsync(request.Id.Value, loggedUser.StoreId.Value);
            ThrowError.Against(qrCodeData == null, "Cannot find QR code information");

            var qrCodeDetail = _mapper.Map<QRCodeDetailDto>(qrCodeData);

            if (qrCodeData.AreaTableId != null)
            {
                var areaTableInfo = await _unitOfWork.AreaTables
                    .Find(x => x.StoreId == loggedUser.StoreId.Value && x.Id == qrCodeData.AreaTableId)
                    .Include(x => x.Area)
                    .Select(x => new
                    {
                        AreaId = x.Area.Id,
                        TableId = x.Id,
                        AreaName = x.Area.Name,
                        TableName = x.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                qrCodeDetail.AreaId = areaTableInfo?.AreaId;
                qrCodeDetail.AreaName = areaTableInfo?.AreaName;
                qrCodeDetail.TableId = areaTableInfo?.TableId;
                qrCodeDetail.AreaTableName = areaTableInfo?.TableName;
            }

            var branchName = await _unitOfWork.StoreBranches
                .Find(x => x.StoreId == loggedUser.StoreId.Value && x.Id == qrCodeData.StoreBranchId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            qrCodeDetail.BranchName = branchName;
            qrCodeDetail.StatusId = _qrCodeService.GetQrCodeStatus(qrCodeDetail.StartDate.Value, qrCodeDetail.EndDate);
            if (qrCodeDetail.StatusId == EnumQRCodeStatus.Scheduled)
            {
                qrCodeDetail.CanDelete = true;
            }
            else
            {
                qrCodeDetail.CanDelete = false;
            }

            return new GetQRCodeDetailByIdRequestResponse
            {
                QrCodeDetail = qrCodeDetail
            };
        }
    }
}
using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using GoFoodBeverage.Models.QRCode;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Queries
{
    public class GetAllQRCodeRequest : IRequest<GetAllQRCodeResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? BranchId { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? TargetId { get; set; }

        public int? StatusId { get; set; }
    }

    public class GetAllQRCodeResponse
    {
        public IEnumerable<QRCodeModel> QRCodes { get; set; }

        public QRCodeFilterModel QRCodeFilters { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetAllQRCodeRequestHandler : IRequestHandler<GetAllQRCodeRequest, GetAllQRCodeResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQrCodeService _qrCodeService;

        public GetAllQRCodeRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IQrCodeService qrCodeService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrCodeService = qrCodeService;
        }

        public async Task<GetAllQRCodeResponse> Handle(GetAllQRCodeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(loggedUser.StoreId == null, "Cannot find store information");

            var listQRCode = new PagingExtensions.Pager<QRCode>(new List<QRCode>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                listQRCode = await _unitOfWork.QRCodes
                .GetAllQRCodeInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = StringHelpers.RemoveSign4VietnameseString(request.KeySearch.Trim().ToLower());
                listQRCode = await _unitOfWork.QRCodes
                .GetAllQRCodeInStore(loggedUser.StoreId.Value)
                .Where(qr => qr.Name.ToLower().Contains(keySearch))
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listQRCodeModels = _mapper.Map<IEnumerable<QRCodeModel>>(listQRCode.Result);

            listQRCodeModels.ForEach(qrCode =>
            {
                if (qrCode.IsStopped)
                {
                    qrCode.StatusId = (int)EnumQRCodeStatus.Finished;
                }
                else
                {
                    qrCode.StatusId = (int)_qrCodeService.GetQrCodeStatus(qrCode.StartDate, qrCode.EndDate);
                }
            });

            ///Initial Filter
            var qrCodeFilter = new QRCodeFilterModel();
            qrCodeFilter.Branches = _unitOfWork.StoreBranches.Find(sb => sb.StoreId == loggedUser.StoreId && sb.IsDeleted == false)
                .Select(c => new QRCodeFilterModel.BranchDto { Id = c.Id, Name = c.Name })
                .ToList();
            qrCodeFilter.ServiceTypes = Enum.GetValues(typeof(EnumOrderType))
                                .Cast<EnumOrderType>()
                                .Where(ot => ot == EnumOrderType.Instore || ot == EnumOrderType.Online)
                                .Select(e => new QRCodeFilterModel.ServiceTypeDto { Id = e })
                                .ToList();
            qrCodeFilter.Targets = Enum.GetValues(typeof(EnumTargetQRCode))
                                .Cast<EnumTargetQRCode>()
                                .Select(e => new QRCodeFilterModel.TargetDto { Id = e })
                                .ToList();
            qrCodeFilter.Status = Enum.GetValues(typeof(EnumQRCodeStatus))
                                .Cast<EnumQRCodeStatus>()
                                .Select(e => new QRCodeFilterModel.StatusDto { Id = e })
                                .ToList();

            ///Handle Filter
            if (listQRCodeModels != null)
            {
                if (request.BranchId != null)
                {
                    listQRCodeModels = listQRCodeModels.Where(qr => qr.StoreBranchId == request.BranchId);
                }

                if (request.ServiceTypeId != null)
                {
                    listQRCodeModels = listQRCodeModels.Where(qr => qr.ServiceTypeId == (EnumOrderType)request.ServiceTypeId);
                }

                if (request.TargetId != null)
                {
                    listQRCodeModels = listQRCodeModels.Where(qr => qr.TargetId == (EnumTargetQRCode)request.TargetId);
                }

                if (request.StatusId != null)
                {
                    listQRCodeModels = listQRCodeModels.Where(qr => qr.StatusId == (int)request.StatusId);
                }
            }

            var response = new GetAllQRCodeResponse()
            {
                QRCodes = listQRCodeModels,
                QRCodeFilters = qrCodeFilter,
                PageNumber = request.PageNumber,
                Total = listQRCode.Total
            };

            return response;
        }
    }
}

using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Commands
{
    public class UpdateQrCodeRequest : IRequest<bool>
    {
        public Guid? QrCodeId { get; set; }

        public Guid? BranchId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public EnumTargetQRCode QrCodeTarget { get; set; }

        public EnumOrderType ServiceTypeId { get; set; }

        public bool IsPercentage { get; set; }

        public Guid AreaId { get; set; }

        public Guid TableId { get; set; }

        public decimal MaxDiscount { get; set; }

        public decimal DiscountValue { get; set; }

        public IEnumerable<QrCodeProductDto> QrCodeProducts { get; set; }

        public class QrCodeProductDto
        {
            public Guid ProductId { get; set; }

            public int ProductQuantity { get; set; }
        }
    }

    public class UpdateQrCodeRequestHandler : IRequestHandler<UpdateQrCodeRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdateQrCodeRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateQrCodeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var qrCode = await _unitOfWork.QRCodes
                .GetAllQRCodeInStore(loggedUser.StoreId.Value)
                .Include(qrCode => qrCode.QRCodeProducts)
                .FirstOrDefaultAsync(qrCode => qrCode.Id == request.QrCodeId);

            ThrowError.BadRequestAgainstNull(qrCode, "Cannot find qr code information");

            var previousData = qrCode.ToJson();

            qrCode.Id = request.QrCodeId.Value;
            qrCode.StoreId = loggedUser.StoreId.Value;
            qrCode.Name = request.Name;
            qrCode.StoreBranchId = request.BranchId.Value;
            qrCode.ServiceTypeId = request.ServiceTypeId;
            qrCode.AreaTableId = request.TableId;
            qrCode.StartDate = request.StartDate.ToUtcDateTime();
            qrCode.EndDate = request.EndDate.ToUtcDateTime();
            qrCode.TargetId = request.QrCodeTarget;
            qrCode.IsPercentDiscount = request.IsPercentage;
            qrCode.PercentNumber = request.IsPercentage == true ? request.DiscountValue : 0;
            qrCode.MaximumDiscountAmount = request.IsPercentage == true ? request.MaxDiscount : request.DiscountValue;

            if (request.QrCodeProducts != null && request.QrCodeProducts.Any())
            {
                _unitOfWork.QrCodeProducts.RemoveRange(qrCode.QRCodeProducts);

                var newQrCodeProducts = new List<QRCodeProduct>();
                foreach (var qrCodeProduct in request.QrCodeProducts)
                {
                    var newQrCodeProduct = new QRCodeProduct()
                    {
                        QRCodeId = qrCode.Id,
                        ProductId = qrCodeProduct.ProductId,
                        ProductQuantity = qrCodeProduct.ProductQuantity
                    };

                    newQrCodeProducts.Add(newQrCodeProduct);
                }

                _unitOfWork.QrCodeProducts.AddRange(newQrCodeProducts);
            }

            _unitOfWork.QRCodes.Update(qrCode);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(previousData, qrCode.ToJson());

            return true;
        }
    }
}

using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Commands
{
    public class CreateQRCodeRequest : IRequest<bool>
    {
        public Guid? ClonedByQrCodeId { get; set; }

        public Guid? QrCodeId { get; set; }

        public Guid? BranchId { get; set; }

        public string Name { get; set; }

        public string QrCodeUrl { get; set; }

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

    public class CreateQRCodeRequestHandler : IRequestHandler<CreateQRCodeRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateQRCodeRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateQRCodeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var newQrCode = new QRCode()
            {
                ClonedByQrCodeId = request.ClonedByQrCodeId,
                Id = request.QrCodeId.Value,
                StoreId = loggedUser.StoreId.Value,
                Name = request.Name,
                StoreBranchId = request.BranchId.Value,
                ServiceTypeId = request.ServiceTypeId,
                AreaTableId = request.TableId,
                Url = request.QrCodeUrl,
                StartDate = request.StartDate.ToUtcDateTime(),
                EndDate = request.EndDate.ToUtcDateTime(),
                TargetId = request.QrCodeTarget,
                IsPercentDiscount = request.IsPercentage,
                PercentNumber = request.IsPercentage == true ? request.DiscountValue : 0,
                MaximumDiscountAmount = request.IsPercentage == true ? request.MaxDiscount : request.DiscountValue,
                IsStopped = false,
                QRCodeProducts = new List<QRCodeProduct>()
            };

            if (request.QrCodeProducts != null && request.QrCodeProducts.Any())
            {
                foreach (var qrCodeProduct in request.QrCodeProducts)
                {
                    var newQrCodeProduct = new QRCodeProduct()
                    {
                        QRCodeId = newQrCode.Id,
                        ProductId = qrCodeProduct.ProductId,
                        ProductQuantity = qrCodeProduct.ProductQuantity
                    };

                    newQrCode.QRCodeProducts.Add(newQrCodeProduct);
                }
            }

            _unitOfWork.QRCodes.Add(newQrCode);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}

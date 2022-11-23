using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Commands
{
    /// <summary>
    /// The order package action request from internal tool
    /// </summary>
    public class OrderPackageActionRequest : IRequest<bool>
    {
        public int OrderPackageId { get; set; }

        public string Action { get; set; }

        public string ContractId { get; set; }

        public string Note { get; set; }

        public string Editor { get; set; }
    }

    public class OrderPackageActionRequestHandler : IRequestHandler<OrderPackageActionRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOnlineStoreMenuService _onlineStoreMenuService;

        public OrderPackageActionRequestHandler(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            IOnlineStoreMenuService onlineStoreMenuService
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _onlineStoreMenuService = onlineStoreMenuService;
        }

        public async Task<bool> Handle(OrderPackageActionRequest request, CancellationToken cancellationToken)
        {
            /// Check the package in the database
            var orderPackage = await _unitOfWork.OrderPackages
                .GetAll()
                .FirstOrDefaultAsync(p => p.Code == request.OrderPackageId, cancellationToken: cancellationToken);

            ThrowError.BadRequestAgainstNull(orderPackage, "Can not found the package");

            /// Update package
            switch (request.Action.Trim().ToUpper())
            {
                case OrderPackageActionConstants.APPROVE:

                    /// Activate order package
                    orderPackage.ContractId = request.ContractId;
                    orderPackage.Note = request.Note;
                    orderPackage.Status = EnumOrderPackageStatus.APPROVED.GetName();
                    orderPackage.LastModifiedDate = _dateTimeService.NowUtc;
                    orderPackage.LastModifiedBy = request.Editor;
                    orderPackage.ExpiredDate = _dateTimeService.NowUtc.AddMonths(orderPackage.PackageDurationByMonth);
                    orderPackage.IsActivated = true;

                    if (orderPackage.OrderPackageType == EnumOrderPackageType.StoreActivate)
                    {
                        await ActivateStoreAsync(orderPackage);
                    }

                    if (orderPackage.OrderPackageType == EnumOrderPackageType.BranchPurchase)
                    {
                        await UpdateOldBranchPurchaseOrderPackageAsync(orderPackage);
                    }

                    break;

                case OrderPackageActionConstants.CANCEL:
                    orderPackage.Status = EnumOrderPackageStatus.CANCELED.GetName();
                    orderPackage.Note = request.Note;
                    orderPackage.LastModifiedDate = _dateTimeService.NowUtc;
                    orderPackage.LastModifiedBy = request.Editor;
                    break;
            }

            _unitOfWork.OrderPackages.Update(orderPackage);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task ActivateStoreAsync(OrderPackage orderPackage)
        {
            var store = await _unitOfWork.Stores.Find(s => s.Id == orderPackage.StoreId).FirstOrDefaultAsync();
            if (store != null)
            {
                store.IsActivated = true;
                store.ActivatedByOrderPackageId = orderPackage.Id;
                _unitOfWork.Stores.Update(store);

                /// Update menu default for Menu management if package WEB approve
                try
                {
                    if (orderPackage.PackageId == EnumPackage.WEB.ToGuid() && orderPackage.StoreId != null)
                    {
                        await _onlineStoreMenuService.CreateDataMenuDefaultAsync(orderPackage.StoreId.Value);
                    } 
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private async Task UpdateOldBranchPurchaseOrderPackageAsync(OrderPackage orderPackage)
        {
            /// Deactivate old branch purchase order packages
            var oldBranchPurchaseOrderPackages = await _unitOfWork.OrderPackages
             .GetAll()
             .Where(op => op.StoreId == orderPackage.StoreId &&
                          op.ActivateStorePackageId == orderPackage.ActivateStorePackageId &&
                          op.IsActivated == true &&
                          op.Status == EnumOrderPackageStatus.APPROVED.GetName())
             .ToListAsync();
            foreach (var oldOrderPackage in oldBranchPurchaseOrderPackages)
            {
                oldOrderPackage.IsActivated = false;
            }

            _unitOfWork.OrderPackages.UpdateRange(oldBranchPurchaseOrderPackages);
        }
    }
}

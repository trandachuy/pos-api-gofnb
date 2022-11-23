using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class CreatePurchaseOrderRequest : IRequest<bool>
    {
        public Guid SupplierId { get; set; }

        public Guid BranchId { get; set; }

        public string Note { get; set; }

        public List<MaterialDto> Materials { get; set; }

        public class MaterialDto
        {
            public Guid Id { get; set; }

            public Guid UnitId { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }
        }
    }

    public class CreatePurchaseOrderRequestHandler : IRequestHandler<CreatePurchaseOrderRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public CreatePurchaseOrderRequestHandler(
           IMediator mediator,
           IUnitOfWork unitOfWork,
           IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var branch = await _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(request.BranchId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(branch == null, "Cannot find store information");

            var supplier = await _unitOfWork.Suppliers
                .Find(b => b.StoreId == loggedUser.StoreId && b.Id == request.SupplierId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(supplier == null, "Cannot find supplier information");

            RequestValidation(request);

            var storeConfig = await _unitOfWork.StoreConfigs.GetStoreConfigByStoreIdAsync(store.Id);
            var newPurchaseOrder = CreatePurchaseOrder(request, store, loggedUser.AccountId.Value, storeConfig.CurrentMaxPurchaseOrderCode);
            _unitOfWork.PurchaseOrders.Add(newPurchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            // update store configure
            await _unitOfWork.StoreConfigs.UpdateStoreConfigAsync(storeConfig, StoreConfigConstants.PURCHASE_ORDER_CODE);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.PurchaseOrder,
                ActionType = EnumActionType.Created,
                ObjectId = newPurchaseOrder.Id,
                ObjectName = newPurchaseOrder.Code
            }, cancellationToken);

            return true;
        }

        private static void RequestValidation(CreatePurchaseOrderRequest request)
        {
            ThrowError.Against(request.Materials.Count == 0, "Please enter material.");
        }

        private static PurchaseOrder CreatePurchaseOrder(CreatePurchaseOrderRequest request, Store store, Guid accountId, int number)
        {
            var newMPurchaseOrder = new PurchaseOrder()
            {
                Code = $"{DefaultConstants.PURCHASE_ORDER_CODE_PREFIX}-{number:0000}",
                StoreId = store.Id,
                SupplierId = request.SupplierId,
                StoreBranchId = request.BranchId,
                Note = request.Note,
                StatusId = EnumPurchaseOrderStatus.Draft,
                CreatedUser = accountId
            };

            var materials = new List<PurchaseOrderMaterial>();
            request.Materials.ForEach(p =>
            {
                var material = new PurchaseOrderMaterial()
                {
                    PurchaseOrderId = newMPurchaseOrder.Id,
                    MaterialId = p.Id,
                    UnitId = p.UnitId,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Total = p.Quantity * p.Price,
                    StoreId = store.Id,
                };
                materials.Add(material);
            });
            newMPurchaseOrder.PurchaseOrderMaterials = materials;

            return newMPurchaseOrder;
        }
    }
}

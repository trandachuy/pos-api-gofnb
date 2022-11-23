using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class UpdatePurchaseOrderByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public UpdatePurchaseOrderByIdDto PurchaseOrderDto { get; set; }

        public class UpdatePurchaseOrderByIdDto
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
    }

    public class UpdatePurchaseOrderRequestHandler : IRequestHandler<UpdatePurchaseOrderByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdatePurchaseOrderRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdatePurchaseOrderByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetPurchaseOrderByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(purchaseOrder == null, "Cannot find purchase order information");

            var branch = await _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(request.PurchaseOrderDto.BranchId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(branch == null, "Cannot find store information");

            var supplier = await _unitOfWork.Suppliers.Find(b => b.StoreId == loggedUser.StoreId && b.Id == request.PurchaseOrderDto.SupplierId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(supplier == null, "Cannot find supplier information");

            ThrowError.Against(purchaseOrder.StatusId != EnumPurchaseOrderStatus.Draft, "Can not modify purchase order in this status");

            RequestValidation(request);

            var modifiedPurchaseOrder = await UpdatePurchaseOrderAsync(purchaseOrder, request, store, loggedUser.AccountId.Value);
            await _unitOfWork.PurchaseOrders.UpdateAsync(modifiedPurchaseOrder);

            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.PurchaseOrder,
                ActionType = EnumActionType.Edited,
                ObjectId = modifiedPurchaseOrder.Id,
                ObjectName = modifiedPurchaseOrder.Code
            });

            return true;
        }

        private static void RequestValidation(UpdatePurchaseOrderByIdRequest request)
        {
            ThrowError.Against(request.PurchaseOrderDto.Materials.Count == 0, "Please enter material.");
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder currentPurchaseOrder, UpdatePurchaseOrderByIdRequest request, Store store, Guid accountId)
        {
            currentPurchaseOrder.SupplierId = request.PurchaseOrderDto.SupplierId;
            currentPurchaseOrder.Note = request.PurchaseOrderDto.Note;
            currentPurchaseOrder.LastSavedUser = accountId;

            var currentPurchaseOrderMaterials = currentPurchaseOrder.PurchaseOrderMaterials.ToList();
            var newPurchaseOrderMaterials = new List<PurchaseOrderMaterial>();
            var deletedPurchaseOrderMaterials = currentPurchaseOrderMaterials.Where(m => !request.PurchaseOrderDto.Materials.Where(x => x.Id != Guid.Empty).Any(x => x.Id == m.MaterialId));

            request.PurchaseOrderDto.Materials.ForEach(material =>
            {
                var purchaseOrderMaterial = currentPurchaseOrderMaterials.FirstOrDefault(m => m.MaterialId == material.Id);
                if (purchaseOrderMaterial == null)
                {
                    //Add new
                    var newMaterial = new PurchaseOrderMaterial()
                    {
                        PurchaseOrderId = currentPurchaseOrder.Id,
                        MaterialId = material.Id,
                        UnitId = material.UnitId,
                        Quantity = material.Quantity,
                        Price = material.Price,
                        Total = material.Quantity * material.Price,
                        CreatedUser = accountId,
                        CreatedTime = DateTime.UtcNow,
                        StoreId = store.Id
                    };
                    newPurchaseOrderMaterials.Add(newMaterial);
                }
                else
                {
                    // update
                    purchaseOrderMaterial.UnitId = material.UnitId;
                    purchaseOrderMaterial.Quantity = material.Quantity;
                    purchaseOrderMaterial.Price = material.Price;
                    purchaseOrderMaterial.Total = material.Quantity * material.Price;
                }
            });

            await _unitOfWork.PurchaseOrderMaterials.AddRangeAsync(newPurchaseOrderMaterials);
            await _unitOfWork.PurchaseOrderMaterials.UpdateRangeAsync(currentPurchaseOrderMaterials);
            await _unitOfWork.PurchaseOrderMaterials.RemoveRangeAsync(deletedPurchaseOrderMaterials);
            return currentPurchaseOrder;
        }
    }
}

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
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class UpdatePurchaseOrderStatusByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public EnumPurchaseOrderAction Action { get; set; }
    }

    public class UpdatePurchaseOrderStatusByIdHandler : IRequestHandler<UpdatePurchaseOrderStatusByIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public UpdatePurchaseOrderStatusByIdHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(UpdatePurchaseOrderStatusByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetPurchaseOrderByIdBackupInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(purchaseOrder == null, "Cannot find purchase order information");

            var modifiedPurchaseOrder = await UpdatePurchaseOrderAsync(purchaseOrder, request, loggedUser.AccountId.Value);

            await _unitOfWork.PurchaseOrders.UpdateAsync(modifiedPurchaseOrder);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder currentPurchaseOrder, UpdatePurchaseOrderStatusByIdRequest request, Guid accountId)
        {
            switch (request.Action)
            {
                case EnumPurchaseOrderAction.Approve:
                    ThrowError.Against(currentPurchaseOrder.StatusId != EnumPurchaseOrderStatus.Draft, "Can not modify purchase order in this status");
                    currentPurchaseOrder.StatusId = EnumPurchaseOrderStatus.Approved;
                    break;
                case EnumPurchaseOrderAction.Cancel:
                    ThrowError.Against(currentPurchaseOrder.StatusId == EnumPurchaseOrderStatus.Completed || currentPurchaseOrder.StatusId == EnumPurchaseOrderStatus.Canceled, "Can not modify purchase order in this status");
                    currentPurchaseOrder.StatusId = EnumPurchaseOrderStatus.Canceled;
                    break;
                case EnumPurchaseOrderAction.Complete:
                    ThrowError.Against(currentPurchaseOrder.StatusId == EnumPurchaseOrderStatus.Completed || currentPurchaseOrder.StatusId == EnumPurchaseOrderStatus.Canceled, "Can not modify purchase order in this status");
                    HandleCompletePurchaseOrder(currentPurchaseOrder);
                    currentPurchaseOrder.RestoreData = BackupMaterialOnCompleted(currentPurchaseOrder);
                    currentPurchaseOrder.StatusId = EnumPurchaseOrderStatus.Completed;
                    break;
                default:
                    //Do something
                    break;
            }

            currentPurchaseOrder.LastSavedUser = accountId;

            return Task.FromResult(currentPurchaseOrder);
        }

        private void HandleCompletePurchaseOrder(PurchaseOrder currentPurchaseOrder)
        {
            var purchaseOrderMaterials = _unitOfWork.PurchaseOrderMaterials
                .GetPurchaseOrderMaterialByPurchaseOrderId(currentPurchaseOrder.Id, currentPurchaseOrder.StoreId)
                .Include(po => po.Unit)
                .ThenInclude(u => u.UnitConversions)
                .Include(po => po.Material)
                .ThenInclude(m => m.Unit)
                .ToList();

            var materials = purchaseOrderMaterials.Select(po => po.Material).ToList();
            var materialInventoryBranches = _unitOfWork.MaterialInventoryBranches.GetAll()
                .Where(inventory => inventory.StoreId == currentPurchaseOrder.StoreId && inventory.BranchId == currentPurchaseOrder.StoreBranchId).ToList();

            var newMaterialInventoryBranches = new List<MaterialInventoryBranch>();
            int position = 0;

            if (purchaseOrderMaterials != null)
            {
                purchaseOrderMaterials.ForEach(po =>
                {
                    var material = materials.FirstOrDefault(m => m.Id == po.MaterialId);

                    var unitCapacity = 1;
                    var unitConversions = _unitOfWork.UnitConversions
                        .Find(u => u.StoreId == currentPurchaseOrder.StoreId && u.UnitId == po.UnitId && u.MaterialId == po.MaterialId).FirstOrDefault();

                    if(unitConversions != null)
                    {
                        unitCapacity = unitConversions.Capacity;
                    }

                    /// Update materials quantity in inventory 
                    /// New quantity = Inventory's quantity + (import quantity * import unit capacity conversion)
                    var newMaterialQuantity = material.Quantity + (po.Quantity * unitCapacity);

                    /// Update material cost
                    /// New cost = ((current cost * inventory's quantity) + (import price * import quantity)) / (new material quantity) 
                    var newMaterialCost = (((material.CostPerUnit ?? 0) * material.Quantity) + (po.Price * po.Quantity)) / newMaterialQuantity;

                    material.Quantity = newMaterialQuantity;
                    material.CostPerUnit = newMaterialCost;


                    ///Update quantity materialInventoryBranches
                    var materialInventory = materialInventoryBranches.FirstOrDefault(inventory => inventory.MaterialId == po.MaterialId);
                    if (materialInventory == null)
                    {
                        var materialInventoryBranches = new MaterialInventoryBranch()
                        {
                            StoreId = currentPurchaseOrder.StoreId,
                            BranchId = currentPurchaseOrder.StoreBranchId,
                            MaterialId = po.MaterialId,
                            Position = position,
                            Quantity = po.Quantity * unitCapacity
                        };
                        position += 1;
                        newMaterialInventoryBranches.Add(materialInventoryBranches);
                    }
                    else
                    {
                        var materialInventoryBranches = new MaterialInventoryBranch()
                        {
                            Id = materialInventory.Id,
                            StoreId = currentPurchaseOrder.StoreId,
                            BranchId = currentPurchaseOrder.StoreBranchId,
                            MaterialId = po.MaterialId,
                            Position = position,
                            Quantity = materialInventory.Quantity + (po.Quantity * unitCapacity)
                        };
                        position += 1;
                        _unitOfWork.MaterialInventoryBranches.UpdateAsync(materialInventoryBranches);
                    }
                });
                _unitOfWork.MaterialInventoryBranches.AddRange(newMaterialInventoryBranches);
                _unitOfWork.Materials.UpdateRange(materials);
            }
        }

        private string BackupMaterialOnCompleted(PurchaseOrder purchaseOrder)
        {
            string jsonData = purchaseOrder.PurchaseOrderMaterials.ToList().ToJson();
            return jsonData;
        }
    }
}

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
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class CompletePurchaseOrderStatusByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class CompletePurchaseOrderStatusByIdRequestHandler : IRequestHandler<CompletePurchaseOrderStatusByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public CompletePurchaseOrderStatusByIdRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(CompletePurchaseOrderStatusByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var purchaseOrder = await _unitOfWork
                .PurchaseOrders
                .Find(p => p.StoreId == loggedUser.StoreId.Value && p.Id == request.Id)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(m => m.Material)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(u => u.Unit).ThenInclude(uc => uc.UnitConversions)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.Against(purchaseOrder == null, "Cannot find purchase order information");
            ThrowError.Against(purchaseOrder.StatusId == EnumPurchaseOrderStatus.Completed || purchaseOrder.StatusId == EnumPurchaseOrderStatus.Canceled, "Can not modify purchase order in this status");

            await HandleCompletePurchaseOrderAsync(purchaseOrder, loggedUser);

            purchaseOrder.RestoreData = BackupMaterialOnCompleted(purchaseOrder);
            purchaseOrder.StatusId = EnumPurchaseOrderStatus.Completed;
            purchaseOrder.LastSavedUser = loggedUser.AccountId.Value;

            _unitOfWork.PurchaseOrders.Update(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.PurchaseOrder,
                ActionType = EnumActionType.Completed,
                ObjectId = purchaseOrder.Id,
                ObjectName = purchaseOrder.Code
            });

            return true;
        }

        private async Task HandleCompletePurchaseOrderAsync(PurchaseOrder currentPurchaseOrder, LoggedUserModel loggedUser)
        {
            var purchaseOrderMaterials = currentPurchaseOrder.PurchaseOrderMaterials.ToList();
            var materials = purchaseOrderMaterials.Select(po => po.Material).ToList();
            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
                .GetAll()
                .Where(inventory => inventory.StoreId == currentPurchaseOrder.StoreId && inventory.BranchId == currentPurchaseOrder.StoreBranchId)
                .ToListAsync();

            var newMaterialInventoryBranches = new List<MaterialInventoryBranch>();
            var materialInventoryHistories = new List<MaterialInventoryHistory>();

            if (purchaseOrderMaterials != null)
            {
                var unitConversions = currentPurchaseOrder.PurchaseOrderMaterials.SelectMany(p => p.Unit.UnitConversions);
                purchaseOrderMaterials.ForEach(purchaseOrderMaterial =>
                {
                    var position = purchaseOrderMaterials.IndexOf(purchaseOrderMaterial) + 1;
                    var material = materials.FirstOrDefault(m => m.Id == purchaseOrderMaterial.MaterialId);

                    var unitCapacity = 1; /// Default value
                    var unitConversion = unitConversions.FirstOrDefault(u => u.MaterialId == purchaseOrderMaterial.MaterialId);
                    if (unitConversion != null)
                    {
                        unitCapacity = unitConversion.Capacity;
                    }

                    /// Update materials quantity in inventory 
                    /// New quantity = Inventory's quantity + (import quantity * import unit capacity conversion)
                    var newMaterialQuantity = material.Quantity + (purchaseOrderMaterial.Quantity * unitCapacity);

                    /// Update material cost
                    /// New cost = ((current cost * inventory's quantity) + (import price * import quantity)) / (new material quantity) 
                    var newMaterialCost = (((material.CostPerUnit ?? 0) * material.Quantity) + (purchaseOrderMaterial.Price * purchaseOrderMaterial.Quantity)) / newMaterialQuantity;

                    material.Quantity = newMaterialQuantity;
                    material.CostPerUnit = newMaterialCost;

                    /// Update quantity materialInventoryBranches
                    var materialInventory = materialInventoryBranches.FirstOrDefault(inventory => inventory.MaterialId == purchaseOrderMaterial.MaterialId);
                    if (materialInventory == null)
                    {
                        /// Calculate the material inventory quantity on branch
                        var materialInventoryQuantity = purchaseOrderMaterial.Quantity * unitCapacity;
                        var materialInventoryBranch = new MaterialInventoryBranch()
                        {
                            StoreId = currentPurchaseOrder.StoreId,
                            BranchId = currentPurchaseOrder.StoreBranchId,
                            MaterialId = purchaseOrderMaterial.MaterialId,
                            Position = position,
                            Quantity = materialInventoryQuantity
                        };

                        newMaterialInventoryBranches.Add(materialInventoryBranch);

                        /// Update material inventory history
                        var materialInventoryHistory = new MaterialInventoryHistory()
                        {
                            OldQuantity = 0,
                            NewQuantity = materialInventoryQuantity,
                            Action = EnumInventoryHistoryAction.ImportGoods,
                            Note = EnumInventoryHistoryAction.ImportGoods.GetNote(),
                            CreatedBy = loggedUser.FullName,
                            MaterialInventoryBranchId = materialInventoryBranch.Id,
                        };

                        materialInventoryHistories.Add(materialInventoryHistory);
                    }
                    else
                    {
                        /// Calculate the material inventory quantity on branch
                        var oldQuantity = materialInventory.Quantity;
                        var materialInventoryQuantity = materialInventory.Quantity + (purchaseOrderMaterial.Quantity * unitCapacity);
                        materialInventory.Position = position;
                        materialInventory.Quantity = materialInventoryQuantity;

                        _unitOfWork.MaterialInventoryBranches.Update(materialInventory);

                        /// Update material inventory history
                        var materialInventoryHistory = new MaterialInventoryHistory()
                        {
                            OldQuantity = oldQuantity,
                            NewQuantity = materialInventoryQuantity,
                            Action = EnumInventoryHistoryAction.ImportGoods,
                            Note = EnumInventoryHistoryAction.ImportGoods.GetNote(),
                            CreatedBy = loggedUser.FullName,
                            MaterialInventoryBranchId = materialInventory.Id,
                        };

                        materialInventoryHistories.Add(materialInventoryHistory);
                    }
                });

                _unitOfWork.MaterialInventoryBranches.AddRange(newMaterialInventoryBranches);
                _unitOfWork.MaterialInventoryHistories.AddRange(materialInventoryHistories);

                _unitOfWork.Materials.UpdateRange(materials);
            }
        }

        private static string BackupMaterialOnCompleted(PurchaseOrder purchaseOrder)
        {
            string jsonData = purchaseOrder.PurchaseOrderMaterials.ToList().ToJson();

            return jsonData;
        }
    }
}

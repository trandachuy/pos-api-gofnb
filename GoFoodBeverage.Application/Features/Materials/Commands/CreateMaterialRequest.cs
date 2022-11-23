using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Unit;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using System.Collections.ObjectModel;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class CreateMaterialRequest : IRequest<bool>
    {
        public string Logo { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? MaterialCategoryId { get; set; }

        public string Sku { get; set; }

        public int? MinQuantity { get; set; }

        public decimal? CostPerUnit { get; set; }

        public bool HasExpiryDate { get; set; }

        public IEnumerable<MaterialInventoryBranchDto> Branches { get; set; }

        /// <summary>
        ///  Material's Quantity each branches
        ///  This class only apply in CreateMaterialRequest
        /// </summary>
        public class MaterialInventoryBranchDto
        {
            public Guid? BranchId { get; set; }

            public int Position { get; set; }

            public int Quantity { get; set; }
        }

        public IEnumerable<UnitConversionDto> UnitConversions { get; set; }
    }

    public class CreateMaterialRequestHandler : IRequestHandler<CreateMaterialRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateMaterialRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateMaterialRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);

            await RequestValidationAsync(request, store.Id);

            var materialNameExisted = await _unitOfWork.Materials.Find(g => g.StoreId == loggedUser.StoreId && g.Name == request.Name).FirstOrDefaultAsync();
            ThrowError.Against(materialNameExisted != null, "Name of material has already existed");

            var newMaterial = CreateMaterial(request, store);
            await _unitOfWork.Materials.AddAsync(newMaterial);
            var materialInventoryBranches = CreateMaterialInventoryBranches(request, store.Id, newMaterial.Id, loggedUser.FullName);
            _unitOfWork.MaterialInventoryBranches.AddRange(materialInventoryBranches);

            /// Insert UnitConversions
            if (request.UnitConversions != null && request.UnitConversions.Any())
            {
                var newUnitConversions = CreateUnitConversions(request, newMaterial, loggedUser.StoreId);
                _unitOfWork.UnitConversions.AddRange(newUnitConversions);
            }

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Material,
                ActionType = EnumActionType.Created,
                ObjectId = newMaterial.Id,
                ObjectName = newMaterial.Name.ToString(),
                ObjectThumbnail = newMaterial.Thumbnail
            });

            return true;
        }

        private static List<UnitConversion> CreateUnitConversions(CreateMaterialRequest request, Material material, Guid? storeId)
        {
            List<UnitConversion> unitConversions = new();
            if (request.UnitConversions.Any())
            {
                foreach (var unitConversion in request.UnitConversions)
                {
                    var newUnitConversion = new UnitConversion()
                    {
                        UnitId = unitConversion.UnitId,
                        Capacity = unitConversion.Capacity,
                        StoreId = storeId,
                        MaterialId = material.Id
                    };

                    unitConversions.Add(newUnitConversion);
                }
            }

            return unitConversions;
        }

        private async Task RequestValidationAsync(CreateMaterialRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter name of material.");
            ThrowError.Against(request.UnitId == null, "Please select unit.");

            var skuExisted = await _unitOfWork.Materials.Find(m => m.StoreId == storeId && m.Sku == request.Sku).FirstOrDefaultAsync();
            ThrowError.Against(request.Sku != null && skuExisted != null, "material.skuExisted");
        }

        private static Material CreateMaterial(CreateMaterialRequest request, Store store)
        {
            var quantity = 0;
            if (request.Branches.Any())
            {
                quantity = request.Branches.Sum(item => item.Quantity);
            }

            var newMaterial = new Material()
            {
                StoreId = store.Id,
                Name = request.Name,
                Description = request.Description,
                UnitId = request.UnitId,
                MaterialCategoryId = request.MaterialCategoryId,
                Sku = request.Sku,
                MinQuantity = request.MinQuantity ?? 0,
                Quantity = quantity,
                CostPerUnit = request.CostPerUnit ?? 0,
                HasExpiryDate = request.HasExpiryDate,
                Thumbnail = request.Logo,
                IsActive = true
            };

            return newMaterial;
        }

        private static List<MaterialInventoryBranch> CreateMaterialInventoryBranches(CreateMaterialRequest request, Guid storeId, Guid materialId, string username)
        {
            var newMaterialInventory = new List<MaterialInventoryBranch>();
            if (request.Branches.Any())
            {
                var branches = request.Branches;
                foreach (var inventory in branches)
                {
                    MaterialInventoryBranch materialInventoryBranchItem = new MaterialInventoryBranch();
                    materialInventoryBranchItem.MaterialInventoryHistories = new Collection<MaterialInventoryHistory>();
                    materialInventoryBranchItem.StoreId = storeId;
                    materialInventoryBranchItem.BranchId = inventory.BranchId;
                    materialInventoryBranchItem.MaterialId = materialId;
                    materialInventoryBranchItem.Position = inventory.Position;
                    materialInventoryBranchItem.Quantity = inventory.Quantity;
                    materialInventoryBranchItem.MaterialInventoryHistories.Add(new MaterialInventoryHistory()
                    {
                        MaterialInventoryBranchId = inventory.BranchId,
                        NewQuantity = inventory.Quantity,
                        OldQuantity = 0,
                        Note = EnumInventoryHistoryAction.UpdateStock.ToString(),
                        Action = EnumInventoryHistoryAction.UpdateStock,
                        CreatedBy = username
                    });

                    newMaterialInventory.Add(materialInventoryBranchItem);
                }
            }

            return newMaterialInventory;
        }
    }
}

using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Unit;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class UpdateMaterialRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? MaterialCategoryId { get; set; }

        public Guid? UnitId { get; set; }

        public string Sku { get; set; }

        public int? MinQuantity { get; set; }

        public bool HasExpiryDate { get; set; }

        public string Logo { get; set; }

        public IEnumerable<MaterialInventoryBranchDto> Branches { get; set; }

        public class MaterialInventoryBranchDto
        {
            public Guid Id { get; set; }

            public Guid BranchId { get; set; }

            public int Position { get; set; }

            public int Quantity { get; set; }
        }

        public IEnumerable<UnitConversionDto> UnitConversions { get; set; }

    }

    public class UpdateMaterialRequestHandler : IRequestHandler<UpdateMaterialRequest, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateMaterialRequestHandler(
            IMapper mapper,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mapper = mapper;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateMaterialRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);

            await RequestValidationAsync(request, store);

            var material = await _unitOfWork.Materials
                .GetAllMaterialsInStore(loggedUser.StoreId)
                .Include(m => m.MaterialInventoryBranches)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);

            var updateMaterial = UpdateMaterial(material, request);
            _unitOfWork.Materials.Update(updateMaterial);

            /// Insert UnitConversions
            if (request.UnitConversions != null && request.UnitConversions.Any())
            {
                var oldUnitConversions = await _unitOfWork.UnitConversions.Find(u => u.StoreId == loggedUser.StoreId && u.MaterialId == material.Id).ToListAsync(cancellationToken: cancellationToken);
                _unitOfWork.UnitConversions.RemoveRange(oldUnitConversions);

                List<UnitConversion> newUnitConversions = new();
                foreach (var unitConversion in request.UnitConversions)
                {
                    var newUnitConversion = new UnitConversion()
                    {
                        UnitId = unitConversion.UnitId,
                        Capacity = unitConversion.Capacity,
                        StoreId = store.Id,
                        MaterialId = material.Id
                    };

                    newUnitConversions.Add(newUnitConversion);
                }

                _unitOfWork.UnitConversions.AddRange(newUnitConversions);
            }

            await _unitOfWork.SaveChangesAsync();

            await UpdateMaterialInventoryBranchesAsync(material, request, loggedUser);

            await _userActivityService.LogAsync(material, updateMaterial);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Material,
                ActionType = EnumActionType.Edited,
                ObjectId = updateMaterial.Id,
                ObjectName = updateMaterial.Name.ToString(),
                ObjectThumbnail = updateMaterial.Thumbnail
            });

            return true;
        }

        private async Task RequestValidationAsync(UpdateMaterialRequest request, Store store)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), "Please enter name of material.");
            ThrowError.Against(request.UnitId == null, "Please select unit.");

            var skuExisted = await _unitOfWork.Materials
               .GetAll()
               .Where(m => m.StoreId == store.Id && m.Id != request.Id)
               .AsNoTracking().FirstOrDefaultAsync(m => m.Sku == request.Sku);
            ThrowError.Against(request.Sku != null && skuExisted != null, "This SKU is already existed");

            var materialNameExisted = await _unitOfWork.Materials
               .GetAll()
               .Where(m => m.StoreId == store.Id && m.Id != request.Id)
               .AsNoTracking().FirstOrDefaultAsync(g => g.Name == request.Name);
            ThrowError.Against(materialNameExisted != null, "Name of material has already existed");
        }

        public static Material UpdateMaterial(Material material, UpdateMaterialRequest request)
        {
            var quantity = 0;
            if (request.Branches.Any())
            {
                quantity = request.Branches.Sum(item => item.Quantity);
            }

            material.Name = request.Name;
            material.Description = request.Description;
            material.MaterialCategoryId = request.MaterialCategoryId;
            material.UnitId = request.UnitId;
            material.Sku = request.Sku;
            material.MinQuantity = request.MinQuantity ?? 0;
            material.HasExpiryDate = request.HasExpiryDate;
            material.Thumbnail = request.Logo;
            material.Quantity = quantity;
            material.IsActive = true;

            return material;
        }

        private async Task UpdateMaterialInventoryBranchesAsync(Material material, UpdateMaterialRequest request, LoggedUserModel loggedUser)
        {
            _unitOfWork.MaterialInventoryBranches.RemoveRange(material.MaterialInventoryBranches);

            #region Delete old inventory histories

            var oldMaterialInventoryBranchesIds = material.MaterialInventoryBranches.Select(x => x.Id)
                .Except(request.Branches.Select(x => x.Id)).ToList();

            var oldInventoryHistories = _unitOfWork.MaterialInventoryHistories
                .Find(x => oldMaterialInventoryBranchesIds.Contains(x.MaterialInventoryBranchId.Value)).ToList();

            _unitOfWork.MaterialInventoryHistories.RemoveRange(oldInventoryHistories);

            #endregion

            var materialInventoryBranches = new List<MaterialInventoryBranch>();
            var materialInventoryHistories = new List<MaterialInventoryHistory>();

            foreach (var materialInventoryRequest in request.Branches)
            {
                var materialInventoryBranch = new MaterialInventoryBranch()
                {
                    StoreId = material.StoreId,
                    BranchId = materialInventoryRequest.BranchId,
                    MaterialId = material.Id,
                    Position = materialInventoryRequest.Position,
                    Quantity = materialInventoryRequest.Quantity
                };

                if (materialInventoryRequest.Id != Guid.Empty)
                {
                    materialInventoryBranch.Id = materialInventoryRequest.Id;
                }

                materialInventoryBranches.Add(materialInventoryBranch);

                #region Update material inventory history

                /// Update old quantity
                if (materialInventoryRequest.Id != Guid.Empty)
                {
                    var oldQuantity = material.MaterialInventoryBranches.Where(x => x.Id == materialInventoryRequest.Id).Select(x => x.Quantity).FirstOrDefault();
                    var newQuantity = materialInventoryRequest.Quantity;

                    if (oldQuantity != newQuantity)
                    {
                        var materialInventoryHistory = new MaterialInventoryHistory()
                        {
                            OldQuantity = oldQuantity,
                            NewQuantity = newQuantity,
                            Action = EnumInventoryHistoryAction.UpdateStock,
                            Note = EnumInventoryHistoryAction.UpdateStock.GetNote(),
                            CreatedBy = loggedUser.FullName,
                            MaterialInventoryBranchId = materialInventoryRequest.Id,
                        };

                        materialInventoryHistories.Add(materialInventoryHistory);
                    }
                }
                else /// Insert new inventory branch
                {
                    var materialInventoryHistory = new MaterialInventoryHistory()
                    {
                        OldQuantity = 0,
                        NewQuantity = materialInventoryRequest.Quantity,
                        Action = EnumInventoryHistoryAction.UpdateStock,
                        Note = EnumInventoryHistoryAction.UpdateStock.GetNote(),
                        CreatedBy = loggedUser.FullName,
                        MaterialInventoryBranchId = materialInventoryBranch.Id,
                    };

                    materialInventoryHistories.Add(materialInventoryHistory);
                }
                #endregion
            }

            _unitOfWork.MaterialInventoryBranches.AddRange(materialInventoryBranches);
            _unitOfWork.MaterialInventoryHistories.AddRange(materialInventoryHistories);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.POS.Models.Material;

namespace GoFoodBeverage.POS.Application.Features.Materials.Queries
{
    public class GetMaterialsFromOrdersCurrentShiftRequest : IRequest<GetMaterialsFromOrdersCurrentShiftResponse>
    {
    }

    public class GetMaterialsFromOrdersCurrentShiftResponse
    {
        public IEnumerable<MaterialsFromOrdersCurrentShiftModel> Materials { get; set; }
    }

    public class GetMaterialsFromOrdersCurrentShiftRequestHandler : IRequestHandler<GetMaterialsFromOrdersCurrentShiftRequest, GetMaterialsFromOrdersCurrentShiftResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetMaterialsFromOrdersCurrentShiftRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetMaterialsFromOrdersCurrentShiftResponse> Handle(GetMaterialsFromOrdersCurrentShiftRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
                .GetMaterialInventoryBranchesByBranchId(loggedUser.StoreId.Value, loggedUser.BranchId.Value)
                .Include(m => m.Material)
                .Where(mib => mib.Material.IsActive.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var materialInventoryChecking = await _unitOfWork.MaterialInventoryCheckings
                .GetMaterialInventoryCheckingByShiftId(loggedUser.StoreId.Value, loggedUser.ShiftId.Value)
                .Include(m => m.Material)
                .Where(mic => mic.Material.IsActive.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var materials = new List<MaterialsFromOrdersCurrentShiftModel>();
            foreach (var materialInventoryBranch in materialInventoryBranches)
            {
                var materialInventoryCheckingExist = materialInventoryChecking.FirstOrDefault(m => m.MaterialId == materialInventoryBranch.MaterialId);
                var originalQuantity = materialInventoryCheckingExist != null ? materialInventoryCheckingExist.InventoryQuantity : materialInventoryBranch.Quantity;
                var material = new MaterialsFromOrdersCurrentShiftModel()
                {
                    Id = materialInventoryBranch.Material.Id,
                    Name = materialInventoryBranch.Material.Name,
                    Sku = materialInventoryBranch.Material.Sku,
                    Quantity = originalQuantity,
                    UnitName = materialInventoryBranch.Material.Unit.Name,
                    Thumbnail = materialInventoryBranch.Material.Thumbnail
                };
                materials.Add(material);
            }

            var orderItems = await _unitOfWork.Orders
                .Find(x => x.StoreId == loggedUser.StoreId && x.StatusId != Domain.Enums.EnumOrderStatus.Draft)
                .Where(o => o.ShiftId == loggedUser.ShiftId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductPrice)
                .ThenInclude(pp => pp.ProductPriceMaterials)
                .ThenInclude(ppm => ppm.Material).ThenInclude(m => m.Unit)
                .Where(o => o.StatusId != Domain.Enums.EnumOrderStatus.Canceled)
                .SelectMany(o => o.OrderItems)
                .ToListAsync();

            var materialsFromOrdersCurrentShift = new List<MaterialsFromOrdersCurrentShiftModel>();
            foreach (var orderItem in orderItems)
            {
                var productPriceMaterials = orderItem.ProductPrice?.ProductPriceMaterials;
                if (productPriceMaterials != null)
                {
                    foreach (var materialItem in productPriceMaterials)
                    {
                        var material = materialItem.Material;
                        var totalMaterialQuantity = materialItem.Quantity * orderItems.Sum(x => x.Quantity);
                        var materialsFromOrdersCurrentShiftExisted = materialsFromOrdersCurrentShift.FirstOrDefault(m => m.Id == material.Id);

                        var quantityMaterial = materials.FirstOrDefault(x => x.Id == material.Id)?.Quantity;
                        var materialsFromOrdersCurrentShiftModel = new MaterialsFromOrdersCurrentShiftModel()
                        {
                            Id = material.Id,
                            Name = material.Name,
                            Sku = material.Sku,
                            Used = totalMaterialQuantity,
                            UnitName = material.Unit.Name,
                            Quantity = quantityMaterial ?? 0,
                            Thumbnail = material.Thumbnail
                        };

                        materialsFromOrdersCurrentShift.Add(materialsFromOrdersCurrentShiftModel);

                    }
                }
            }

            var materialsRemaining = materials.Where(m => !materialsFromOrdersCurrentShift.Any(i => i.Id == m.Id)).ToList();
            var materialResponse = materialsFromOrdersCurrentShift.Concat(materialsRemaining).ToList();

            var response = new GetMaterialsFromOrdersCurrentShiftResponse()
            {
                Materials = materialResponse
            };

            return response;
        }

    }
}

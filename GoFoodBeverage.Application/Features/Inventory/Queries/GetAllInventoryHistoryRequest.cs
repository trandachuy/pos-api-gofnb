using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Inventory;
using GoFoodBeverage.Models.Unit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Inventory.Queries
{
    public class GetAllInventoryHistoryRequest : IRequest<GetAllInventoryHistoryResponse>
    {
        public string keySearch { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public Guid? MaterialId { get; set; }

        public Guid? BranchId { get; set; }

        public EnumInventoryHistoryAction? Action { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class GetAllInventoryHistoryResponse
    {
        public IEnumerable<MaterialInventoryHistoryModel> MaterialInventoryHistories { get; set; }

        public int Total { get; set; }
    }

    public class GetAllInventoryHistoryHandler : IRequestHandler<GetAllInventoryHistoryRequest, GetAllInventoryHistoryResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllInventoryHistoryHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllInventoryHistoryResponse> Handle(GetAllInventoryHistoryRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            var materialInventoryBranches = new List<Domain.Entities.MaterialInventoryBranch>();
            if (request.MaterialId.HasValue)
            {
                materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
                .Find(x => x.StoreId == loggedUser.StoreId && x.MaterialId == request.MaterialId)
                .Include(x => x.MaterialInventoryHistories)
                .Include(x => x.Branch)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            }
            else
            {
                materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
                .Find(x => x.StoreId == loggedUser.StoreId)
                .Include(x => x.MaterialInventoryHistories)
                .Include(x => x.Branch)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            }

            if (request.BranchId.HasValue)
            {
                materialInventoryBranches = materialInventoryBranches.Where(x => x.BranchId == request.BranchId.Value).ToList();
            }

            var listMaterialInventoryHistory = new List<MaterialInventoryHistoryModel>();
            var materialIds = materialInventoryBranches.Select(x => x.MaterialId).Distinct().ToList();
            var listMaterial = await _unitOfWork.Materials.Find(x => materialIds.Contains(x.Id))
                                                    .Include(x => x.Unit).ThenInclude(x => x.UnitConversions)
                                                    .Select(x => new { x.Name, x.Unit, x.IsActive, x.Id })
                                                    .ToListAsync(cancellationToken);

            foreach (var item in materialInventoryBranches)
            {
                var material = listMaterial.FirstOrDefault(x => x.Id == item.MaterialId);
                var materialName = material?.Name;
                var baseUnitName = material?.Unit?.Name;

                var unitConversion = await _unitOfWork.UnitConversions
                                                .GetUnitConversionsByMaterialIdInStore(item.MaterialId.Value, loggedUser.StoreId.Value)
                                                .AsNoTracking()
                                                .Include(u => u.Unit)
                                                .Select(x => new UnitConversion { Id = x.Id, Capacity = x.Capacity, UnitId = x.UnitId, MaterialId = x.MaterialId, Unit = x.Unit })
                                                .ToListAsync();

                var unitConversionDetail = _mapper.Map<List<UnitConversionUnitDto>>(unitConversion);
                var materialInventoryHistories = item.MaterialInventoryHistories.Select(x => new MaterialInventoryHistoryModel
                {
                    Action = x.Action.GetName(),
                    CreatedBy = x.CreatedBy,
                    Note = x.Note,
                    Time = x.CreatedTime,
                    MaterialName = materialName,
                    BaseUnitName = baseUnitName,
                    BranchName = item?.Branch?.Name,
                    OldQuantity = x.OldQuantity,
                    NewQuantity = x.NewQuantity,
                    UnitConversion = unitConversionDetail.Select(y => new UnitConversionModel { UnitName = y.Unit.Name, Quantity = Math.Round(x.NewQuantity / y.Capacity, 5) }),
                    Reference = x.Reference,
                    IsActive = material?.IsActive,
                    MaterialId = material?.Id,
                    ActionColor = x.Action.GetColor(),
                    ActionBackgroundColor = x.Action.GetBackgroundColor(),
                    OrderId = x.OrderId
                });
                listMaterialInventoryHistory.AddRange(materialInventoryHistories);
            }

            if (request.Action.HasValue)
            {
                listMaterialInventoryHistory = listMaterialInventoryHistory.Where(x => x.Action == request.Action.Value.GetName()).ToList();
            }

            if (request.IsActive.HasValue)
            {
                listMaterialInventoryHistory = listMaterialInventoryHistory.Where(x => x.IsActive == request.IsActive).ToList();
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var startDate = request.StartDate.Value.StartOfDay().ToUniversalTime();
                var endDate = request.EndDate.Value.EndOfDay().ToUniversalTime();
                listMaterialInventoryHistory = listMaterialInventoryHistory.Where(x => x.Time.Value >= startDate && x.Time.Value <= endDate).ToList();
            }
            else
            {
                listMaterialInventoryHistory = listMaterialInventoryHistory.Where(x => x.Time.Value.Date == DateTime.UtcNow.Date).ToList();
            }

            if (!string.IsNullOrEmpty(request.keySearch))
            {
                listMaterialInventoryHistory = listMaterialInventoryHistory.Where(x => x.MaterialName.Contains(request.keySearch)).ToList();
            }

            var listMaterialInventoryHistoryPaging = listMaterialInventoryHistory.OrderByDescending(x => x.Time).ToList()
                .ToPagination(request.PageNumber, request.PageSize).Result;
            var response = new GetAllInventoryHistoryResponse()
            {
                MaterialInventoryHistories = listMaterialInventoryHistoryPaging,
                Total = listMaterialInventoryHistory.Count
            };

            return response;
        }

    }
}

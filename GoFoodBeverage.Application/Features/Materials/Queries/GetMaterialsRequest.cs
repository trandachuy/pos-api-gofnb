using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialsRequest : IRequest<GetMaterialsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetMaterialsResponse
    {
        public IEnumerable<MaterialModel> Materials { get; set; }

        public int Total { get; set; }

        public decimal? TotalCostAllMaterial { get; set; }
    }

    public class GetMaterialRequestHandler : IRequestHandler<GetMaterialsRequest, GetMaterialsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetMaterialRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetMaterialsResponse> Handle(GetMaterialsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materials = await _unitOfWork.Materials
                .GetAllMaterialsInStore(loggedUser.StoreId)
                .Include(m => m.Unit)
                .OrderByDescending(p => p.CreatedTime)
                .AsNoTracking()
                .ToListAsync();

            var listMaterialIds = materials.Select(x => x.Id).ToList();
            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
              .Find(x=> x.StoreId == loggedUser.StoreId && listMaterialIds.Contains(x.MaterialId.Value))
              .Where(mib => mib.Material.IsActive.Value)
              .Select(x=> new {x.MaterialId, x.Quantity})
              .AsNoTracking()
              .ToListAsync(cancellationToken: cancellationToken);

            decimal totalCostAllMaterial = 0;
            foreach (var material in materials)
            {
                material.Quantity = materialInventoryBranches.Where(x => x.MaterialId == material.Id).Sum(x => x.Quantity);
                if (material.Quantity.HasValue && material.CostPerUnit.HasValue)
                {
                    totalCostAllMaterial = totalCostAllMaterial + material.Quantity.Value * material.CostPerUnit.Value;
                }
            }

            if (!string.IsNullOrEmpty(request.KeySearch) && materials != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                materials = materials.Where(g => g.Name.ToLower().Contains(keySearch) || (!string.IsNullOrEmpty(g.Sku) && g.Sku.ToLower().Contains(keySearch))).ToList();
            }

            var materialsByPaging = materials.ToPagination(request.PageNumber, request.PageSize);
            var materialtModels = _mapper.Map<IEnumerable<MaterialModel>>(materialsByPaging.Result);

            var response = new GetMaterialsResponse()
            {
                Materials = materialtModels,
                Total = materialsByPaging.Total,
                TotalCostAllMaterial = totalCostAllMaterial
            };

            return response;
        }
    }
}

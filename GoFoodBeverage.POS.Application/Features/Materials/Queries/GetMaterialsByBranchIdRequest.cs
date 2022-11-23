using System;
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
    public class GetMaterialsByBranchIdRequest : IRequest<GetMaterialsByBranchIdResponse>
    {
        public Guid BranchId { get; set; }
    }

    public class GetMaterialsByBranchIdResponse
    {
        public IEnumerable<MaterialModel> Materials { get; set; }
    }

    public class GetMaterialsByBranchIdRequestHandler : IRequestHandler<GetMaterialsByBranchIdRequest, GetMaterialsByBranchIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetMaterialsByBranchIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetMaterialsByBranchIdResponse> Handle(GetMaterialsByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
                .GetMaterialInventoryBranchesByBranchId(loggedUser.StoreId.Value, request.BranchId)
                .Include(m => m.Material)
                .Where(mib => mib.Material.IsActive.Value)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var materials = new List<MaterialModel>();
            foreach (var materialInventoryBranch in materialInventoryBranches)
            {
                var material = new MaterialModel()
                {
                    Id = materialInventoryBranch.Material.Id,
                    Name = materialInventoryBranch.Material.Name,
                    Sku = materialInventoryBranch.Material.Sku,
                    Quantity = materialInventoryBranch.Quantity,
                    UnitName = materialInventoryBranch.Material.Unit.Name,
                    Thumbnail = materialInventoryBranch.Material.Thumbnail
                };
                materials.Add(material);

            }

            var response = new GetMaterialsByBranchIdResponse()
            {
                Materials = materials
            };

            return response;
        }

    }
}

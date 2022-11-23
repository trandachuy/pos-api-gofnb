using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Material;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialsByFilterRequest : IRequest<GetMaterialsByFilterResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? MaterialCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class GetMaterialsByFilterResponse
    {
        public IEnumerable<MaterialModel> Materials { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetMaterialsByFilterRequestHandler : IRequestHandler<GetMaterialsByFilterRequest, GetMaterialsByFilterResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMaterialsByFilterRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GetMaterialsByFilterResponse> Handle(GetMaterialsByFilterRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var materials = _unitOfWork.Materials
                .GetAllMaterialsInStore(loggedUser.StoreId.Value);

            if (materials != null)
            {
                if (request.MaterialCategoryId != null)
                {
                    /// Find materials by material categoryId
                    materials = materials.Where(m => m.MaterialCategoryId == request.MaterialCategoryId);
                }

                if (request.BranchId != null)
                {
                    /// Find materials by branchId
                    var materialIdsInMaterialInventoryBranch = _unitOfWork.MaterialInventoryBranches
                        .Find(m => m.StoreId == loggedUser.StoreId && m.BranchId == request.BranchId)
                        .Select(m => m.MaterialId);

                    materials = materials.Where(x => materialIdsInMaterialInventoryBranch.Contains(x.Id));
                }

                if (request.UnitId != null)
                {
                    materials = materials.Where(m => m.UnitId == request.UnitId);
                }

                if (request.IsActive != null)
                {
                    materials = materials.Where(m => m.IsActive == request.IsActive);
                }

                if (!string.IsNullOrEmpty(request.KeySearch))
                {
                    string keySearch = request.KeySearch.Trim().ToLower();
                    materials = materials.Where(g => g.Name.ToLower().Contains(keySearch) || g.Sku.ToLower().Contains(keySearch));
                }
            }

            var allMaterialsInStore = await materials
                .Include(m => m.Unit)
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            var pagingResult = allMaterialsInStore.Result;
            var materialListResponse = _mapper.Map<List<MaterialModel>>(pagingResult);
            materialListResponse.ForEach(m =>
            {
                var material = pagingResult.FirstOrDefault(i => i.Id == m.Id);
            });

            var response = new GetMaterialsByFilterResponse()
            {
                PageNumber = request.PageNumber,
                Total = allMaterialsInStore.Total,
                Materials = materialListResponse
            };

            return response;
        }
    }
}

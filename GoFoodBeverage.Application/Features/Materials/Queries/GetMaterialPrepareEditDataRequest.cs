using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using System;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Unit;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Models.Store;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialPrepareEditDataRequest : IRequest<GetMaterialPrepareEditDataResponse>
    {
        public Guid Materiald { get; set; }
    }

    public class GetMaterialPrepareEditDataResponse
    {
        public IEnumerable<UnitModel> Units { get; set; }

        public IEnumerable<MaterialCategoryModel> MaterialCategories { get; set; }

        public IEnumerable<StoreBranchModel> Branches { get; set; }

        public MaterialByIdModel Material { get; set; }

        public IEnumerable<UnitConversionUnitDto> UnitConversions { get; set; }
    }

    public class GetMaterialPrepareEditDataRequestHandler : IRequestHandler<GetMaterialPrepareEditDataRequest, GetMaterialPrepareEditDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetMaterialPrepareEditDataRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetMaterialPrepareEditDataResponse> Handle(GetMaterialPrepareEditDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var units = await _unitOfWork.Units
                .GetAllUnitsInStore(loggedUser.StoreId)
                .OrderByDescending(u => u.Position)
                .AsNoTracking()
                .ProjectTo<UnitModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var materialCategories = await _unitOfWork.MaterialCategories
                .GetAllMaterialCategoriesInStore(loggedUser.StoreId.Value)
                .OrderBy(m => m.CreatedTime)
                .AsNoTracking()
                .ProjectTo<MaterialCategoryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var branches = await _unitOfWork.StoreBranches
              .GetStoreBranchesByStoreId(loggedUser.StoreId)
              .AsNoTracking()
              .OrderBy(b => b.CreatedTime)
              .ProjectTo<StoreBranchModel>(_mapperConfiguration)
              .ToListAsync(cancellationToken: cancellationToken);

            var materialByIdResponse = await _mediator.Send(new GetMaterialByIdRequest() { Id = request.Materiald }, cancellationToken);

            var response = new GetMaterialPrepareEditDataResponse()
            {
                Units = units,
                Branches = branches,
                Material = materialByIdResponse.Material,
                MaterialCategories = materialCategories,
                UnitConversions = materialByIdResponse.UnitConversions
            };

            return response;
        }
    }
}

using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Queries
{
    public class GetPurchaseOrderPrepareDataRequest : IRequest<GetPurchaseOrderPrepareDataResponse>
    {
    }

    public class GetPurchaseOrderPrepareDataResponse
    {
        public IEnumerable<SupplierModel> Suppliers { get; set; }

        public IEnumerable<StoreBranchModel> Branches { get; set; }

        public IEnumerable<MaterialModel> Materials { get; set; }
    }

    public class GetPurchaseOrderPrepareDataHandler : IRequestHandler<GetPurchaseOrderPrepareDataRequest, GetPurchaseOrderPrepareDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetPurchaseOrderPrepareDataHandler(
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

        public async Task<GetPurchaseOrderPrepareDataResponse> Handle(GetPurchaseOrderPrepareDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var suppliers = await _unitOfWork.Suppliers
               .GetAllSuppliersInStore(loggedUser.StoreId)
               .AsNoTracking()
               .ProjectTo<SupplierModel>(_mapperConfiguration)
               .ToListAsync(cancellationToken: cancellationToken);

            var branches = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(loggedUser.StoreId)
                .ProjectTo<StoreBranchModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var materials = await _unitOfWork.Materials
                .GetAllMaterialsActivatedInStore(loggedUser.StoreId)
                .Include(a => a.MaterialInventoryBranches)
                .Include(x=>x.Unit)
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken: cancellationToken);
            var materialMapping = _mapper.Map<List<MaterialPrepareDataModel>>(materials);

            var response = new GetPurchaseOrderPrepareDataResponse()
            {
                Suppliers = suppliers,
                Branches = branches,
                Materials = materialMapping
            };

            return response;
        }
    }
}

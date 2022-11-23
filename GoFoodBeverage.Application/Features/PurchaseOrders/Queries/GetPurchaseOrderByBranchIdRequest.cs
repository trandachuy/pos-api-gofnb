using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;
using GoFoodBeverage.Models.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Queries
{
    public class GetPurchaseOrderByBranchIdRequest : IRequest<GetPurchaseOrderByBranchIdResponse>
    {
        public Guid BranchId { get; set; }
    }

    public class GetPurchaseOrderByBranchIdResponse
    {
        public List<GetPurchaseOrderByBranchModel> PurchaseOrders { get; set; }

        public bool IsOpenPurchaseOrder { get; set; }

        public StoreBranchModel Branch { get; set; }
    }

    public class GetPurchaseOrderByBranchIdHandler : IRequestHandler<GetPurchaseOrderByBranchIdRequest, GetPurchaseOrderByBranchIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetPurchaseOrderByBranchIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetPurchaseOrderByBranchIdResponse> Handle(GetPurchaseOrderByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var branchData = await _unitOfWork.StoreBranches.GetStoreBranchByStoreIdAndBranchIdAsync(loggedUser.StoreId.Value, request.BranchId)
                .AsNoTracking().ProjectTo<StoreBranchModel>(_mapperConfiguration)
                .FirstOrDefaultAsync();

            var purchaseOrderData = await _unitOfWork.PurchaseOrders.GetAllPurchaseOrderByBranchAsync(loggedUser.StoreId.Value, request.BranchId)
                .AsNoTracking()
                .ProjectTo<GetPurchaseOrderByBranchModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);
            var result = new List<GetPurchaseOrderByBranchModel>();
            var isOpenPO = false;

            foreach (var item in purchaseOrderData)
            {
                if (item.StatusId == EnumPurchaseOrderStatus.Draft || item.StatusId == EnumPurchaseOrderStatus.Approved)
                {
                    isOpenPO = true;
                    result.Add(item);
                }
            }

            return new GetPurchaseOrderByBranchIdResponse
            {
                PurchaseOrders = result,
                IsOpenPurchaseOrder = isOpenPO,
                Branch = branchData
            };
        }
    }
}

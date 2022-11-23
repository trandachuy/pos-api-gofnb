using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Queries
{
    public class GetPurchaseOrderBySupplierIdRequest : IRequest<GetPurchaseOrderBySupplierIdResponse>
    {
        public Guid SupplierId { get; set; }
    }

    public class GetPurchaseOrderBySupplierIdResponse
    {
        public SupplierModel Supplier { get; set; }

        public List<GetPurchaseOrderByIdModel> PurchaseOrders { get; set; }

        public bool IsOpenPurchaseOrder { get; set; }
    }

    public class GetPurchaseOrderBySupplierIdHandler : IRequestHandler<GetPurchaseOrderBySupplierIdRequest, GetPurchaseOrderBySupplierIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPurchaseOrderBySupplierIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetPurchaseOrderBySupplierIdResponse> Handle(GetPurchaseOrderBySupplierIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var supplierData = await _unitOfWork.Suppliers.GetSupplierByIdInStoreAsync(request.SupplierId, loggedUser.StoreId.Value);
            var purchaseOrderData = await _unitOfWork.PurchaseOrders.GetAllPurchaseOrderBySupplierIdInStoreAsync(request.SupplierId, loggedUser.StoreId.Value);

            var supplier = _mapper.Map<SupplierModel>(supplierData);
            var purchaseOrder = _mapper.Map<List<GetPurchaseOrderByIdModel>>(purchaseOrderData);
            var isOpenPO = false;
            var result = new List<GetPurchaseOrderByIdModel>();

            foreach (var item in purchaseOrder)
            {
                if ((item.StatusId == EnumPurchaseOrderStatus.Draft || item.StatusId == EnumPurchaseOrderStatus.Approved)
                    && item.Supplier != null)
                {
                    isOpenPO = true;
                    result.Add(item);
                }
            }

            return new GetPurchaseOrderBySupplierIdResponse
            {
                Supplier = supplier,
                PurchaseOrders = result,
                IsOpenPurchaseOrder = isOpenPO
            };
        }
    }
}

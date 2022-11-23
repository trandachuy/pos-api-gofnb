using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class GetPurchaseOrderRequest : IRequest<GetPurchaseOrderResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetPurchaseOrderResponse
    {
        public IEnumerable<PurchaseOrderModel> PurchaseOrders { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetPurchaseOrderHandler : IRequestHandler<GetPurchaseOrderRequest, GetPurchaseOrderResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPurchaseOrderHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetPurchaseOrderResponse> Handle(GetPurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var purchaseOrderData = await _unitOfWork.PurchaseOrders.GetAllPurchaseOrderByStoreId(loggedUser.StoreId.Value, request.PageNumber, request.PageSize, null);
            var purchaseOrders = _mapper.Map<List<PurchaseOrderModel>>(purchaseOrderData.Result);
            var staffInfo = await _unitOfWork.Staffs.GetStaffByAccountIdAsync(loggedUser.AccountId.Value);
            if (!string.IsNullOrEmpty(request.KeySearch) && purchaseOrders != null)
            {
                string keySearch = StringHelpers.RemoveSign4VietnameseString(request.KeySearch).Trim().ToLower();
                purchaseOrders = purchaseOrders.Where(g => StringHelpers.RemoveSign4VietnameseString(g.Supplier.Name.ToLower()).Contains(keySearch)).ToList();
            }
            purchaseOrders.ForEach(purchaseOrder =>
            {
                purchaseOrder.CreatedBy = staffInfo.FullName;
            });

            var response = new GetPurchaseOrderResponse()
            {
                PageNumber = request.PageNumber,
                Total = purchaseOrderData.Total,
                PurchaseOrders = purchaseOrders
            };

            return response;
        }
    }
}

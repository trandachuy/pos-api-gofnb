using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;
using GoFoodBeverage.Models.Unit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Queries
{
    public class GetPurchaseOrderByIdRequest : IRequest<GetPurchaseOrderByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetPurchaseOrderByIdResponse
    {
        public bool IsSuccess { get; set; }

        public GetPurchaseOrderByIdModel PurchaseOrder { get; set; }
    }

    public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdRequest, GetPurchaseOrderByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetPurchaseOrderByIdHandler(
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

        public async Task<GetPurchaseOrderByIdResponse> Handle(GetPurchaseOrderByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var purchaseOrderData = await _unitOfWork.PurchaseOrders.GetPurchaseOrderByIdInStoreAsync(request.Id.Value, loggedUser.StoreId.Value);
            if (purchaseOrderData.StatusId == EnumPurchaseOrderStatus.Completed)
            {
                purchaseOrderData.PurchaseOrderMaterials = purchaseOrderData.RestoreData.ToObject<List<PurchaseOrderMaterial>>();
            }
            var purchaseOrder = _mapper.Map<GetPurchaseOrderByIdModel>(purchaseOrderData);

            var unitConvertions = await _unitOfWork.UnitConversions
                .GetAllUnitConversionsInStore(loggedUser.StoreId)
                .Include(u => u.Unit)
                .AsNoTracking()
                .ProjectTo<UnitConversionUnitDto>(_mapperConfiguration)
                .ToListAsync();

            foreach (var item in purchaseOrder.PurchaseOrderMaterials)
            {
                item.UnitConversionUnits = unitConvertions.Where(u => u.MaterialId == item.MaterialId).ToList();
            }

            return new GetPurchaseOrderByIdResponse
            {
                IsSuccess = true,
                PurchaseOrder = purchaseOrder
            };
        }
    }
}

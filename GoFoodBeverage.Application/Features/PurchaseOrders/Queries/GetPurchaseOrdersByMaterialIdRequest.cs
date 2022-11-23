using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.PurchaseOrderModel;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Queries
{
    public class GetPurchaseOrdersByMaterialIdRequest : IRequest<GetPurchaseOrdersByMaterialIdResponse>
    {
        public Guid MaterialId { get; set; }
    }

    public class GetPurchaseOrdersByMaterialIdResponse
    {
        public MaterialByIdModel Material { get; set; }

        public List<GetPurchaseOrderByIdModel> PurchaseOrders { get; set; }

        public bool IsOpenPurchaseOrder { get; set; }

        public bool IsOpenProduct { get; set; }

        public List<ProductByMaterialModel> Products { get; set; }
    }

    public class GetPurchaseOrdersByMaterialIdHandler : IRequestHandler<GetPurchaseOrdersByMaterialIdRequest, GetPurchaseOrdersByMaterialIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPurchaseOrdersByMaterialIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetPurchaseOrdersByMaterialIdResponse> Handle(GetPurchaseOrdersByMaterialIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var materialData = await _unitOfWork.Materials.GetMaterialByIdInStoreAsync(request.MaterialId, loggedUser.StoreId.Value);
            var purchaseOrderData = await _unitOfWork.PurchaseOrders.GetAllPurchaseOrderByMaterialIdInStoreAsync(request.MaterialId, loggedUser.StoreId.Value);

            var material = _mapper.Map<MaterialByIdModel>(materialData);
            var purchaseOrder = _mapper.Map<List<GetPurchaseOrderByIdModel>>(purchaseOrderData);
            var isOpenPO = false;
            var isOpenProduct = false;
            var result = new List<GetPurchaseOrderByIdModel>();
            var listProducts = new List<ProductByMaterialModel>();

            foreach (var item in purchaseOrder)
            {
                if ((item.StatusId == EnumPurchaseOrderStatus.Draft || item.StatusId == EnumPurchaseOrderStatus.Approved)
                    && item.PurchaseOrderMaterials.Any())
                {
                    isOpenPO = true;
                    result.Add(item);
                }
            }

            if(!isOpenPO)
            {
                listProducts = await _unitOfWork.ProductPriceMaterials.Find(x => x.StoreId == loggedUser.StoreId && x.MaterialId == request.MaterialId)
                                                                      .Include(x => x.ProductPrice)
                                                                      .ThenInclude(x => x.Product)
                                                                      .Select(x => new ProductByMaterialModel { Id = (x.ProductPrice == null || x.ProductPrice.Product==null ? null: x.ProductPrice.Product.Id), 
                                                                                                                Name = (x.ProductPrice == null || x.ProductPrice.Product == null ? string.Empty : x.ProductPrice.Product.Name) })
                                                                      .Distinct()
                                                                      .OrderBy(x => x.Name)
                                                                      .ToListAsync(cancellationToken: cancellationToken);
                if (listProducts.Count > 0) { isOpenProduct = true; }
            }

            return new GetPurchaseOrdersByMaterialIdResponse
            {
                Material = material,
                PurchaseOrders = result,
                IsOpenPurchaseOrder = isOpenPO,
                Products = listProducts,
                IsOpenProduct = isOpenProduct
            };
        }
    }
}

using AutoMapper;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderSoldProductRequest : IRequest<GetOrderSoldProductResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string SortNo { get; set; }

        public string SortProductName { get; set; }

        public string SortCategory { get; set; }

        public string SortQuantity { get; set; }

        public string SortAmount { get; set; }

        public string SortCost { get; set; }
    }

    public class GetOrderSoldProductResponse
    {
        public IEnumerable<OrderTopSellingProductModel> ListSoldProduct { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalCost { get; set; }
    }

    public class GetOrderSoldProductHandler : IRequestHandler<GetOrderSoldProductRequest, GetOrderSoldProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetOrderSoldProductHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetOrderSoldProductResponse> Handle(GetOrderSoldProductRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            DateTime startDate = request.StartDate;
            DateTime endDate = request.EndDate.AddDays(1).AddSeconds(-1);
            var listOrder = await _unitOfWork.Orders.Find(o => o.StoreId == loggedUser.StoreId
                                                         && o.CreatedTime.Value.CompareTo(startDate) >= 0
                                                         && endDate.CompareTo(o.CreatedTime.Value) >= 0
                                                         && o.StatusId != EnumOrderStatus.Canceled && o.StatusId != EnumOrderStatus.Draft)
                                                    .Select(x => new { x.Id, x.CreatedTime, x.StatusId, x.BranchId, x.CustomerId, x.PriceAfterDiscount })
                                                    .AsNoTracking()
                                                    .ToListAsync(cancellationToken: cancellationToken);

            if (request.BranchId.HasValue)
            {
                listOrder = listOrder.Where(o => o.BranchId == request.BranchId).ToList();
            }

            var listOrderIds = listOrder.Select(x => x.Id);
            var listOrderItems = await _unitOfWork.OrderItems.Find(x => x.StoreId == loggedUser.StoreId && listOrderIds.Contains(x.OrderId.Value))
                                                             .Include(i => i.OrderComboItem)
                                                             .ThenInclude(x => x.OrderComboProductPriceItems)
                                                             .Where(x => x.StatusId != EnumOrderItemStatus.Canceled)
                                                             .Select(x => new { x.IsCombo, x.ProductPriceId, x.OrderComboItem, x.Quantity })
                                                             .ToListAsync(cancellationToken: cancellationToken);
            var listAllProductPrices = new List<ProductPriceGroupModel>();
            var listNotOrderComboItems = listOrderItems.Where(x => x.IsCombo == false)
                                                       .GroupBy(x => new { x.ProductPriceId })
                                                       .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Sum(x => x.Quantity) });
            listAllProductPrices.AddRange(listNotOrderComboItems);
            var listOrderComboItems = listOrderItems.Where(x => x.IsCombo == true)
                                                    .Select(x => x.OrderComboItem)
                                                    .ToList();
            foreach (var orderComboItem in listOrderComboItems)
            {
                var listProductPriceForCombos = orderComboItem.OrderComboProductPriceItems
                                                              .GroupBy(x => new { x.ProductPriceId })
                                                              .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Count() });
                listAllProductPrices.AddRange(listProductPriceForCombos);
            }
            var listAllProductPricesGroup = listAllProductPrices.GroupBy(x => new { x.ProductPriceId })
                                                                .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Sum(x => x.Quantity) })
                                                                .ToList();
            var listProductPriceIds = listAllProductPricesGroup.Select(x => x.ProductPriceId);
            var listProductPrice = await _unitOfWork.ProductPrices.Find(x => x.StoreId == loggedUser.StoreId && listProductPriceIds.Contains(x.Id))
                                                                  .Include(x => x.Product).ThenInclude(x=>x.ProductProductCategories).ThenInclude(x=>x.ProductCategory)
                                                                  .Select(x => new { x.Id, x.PriceName, x.PriceValue, x.Product })
                                                                  .ToListAsync(cancellationToken: cancellationToken);
            var listTopSellingProduct = new List<OrderTopSellingProductModel>();
            var listProductIdsTemp = new List<Guid?>();
            var listProductPriceMaterials = _unitOfWork.ProductPriceMaterials.Find(x => x.StoreId == loggedUser.StoreId && listProductPriceIds.Contains(x.ProductPriceId))
                                                                             .Include(x => x.Material)
                                                                             .Select(x => new {x.ProductPriceId, Cost =  x.Quantity * x.Material.CostPerUnit });

            var listProductCost = listProductPriceMaterials.GroupBy(x => new { x.ProductPriceId })
                                                                .Select(g => new  { ProductPriceId = g.Key.ProductPriceId, ProductCost = g.Sum(x => x.Cost) })
                                                                .ToList();
           
            var no = 1;

            foreach (var productPriceItem in listAllProductPricesGroup)
            {
                var topSellingProduct = new OrderTopSellingProductModel();
                var productPrice = listProductPrice.FirstOrDefault(x => x.Id == productPriceItem.ProductPriceId);
                var productCost = listProductCost.FirstOrDefault(x => x.ProductPriceId == productPriceItem.ProductPriceId);

                if (!listProductIdsTemp.Contains(productPrice?.Product?.Id))
                {
                    listProductIdsTemp.Add(productPrice?.Product?.Id);
                }
                else
                {
                    continue;
                }
                topSellingProduct.No = no;
                topSellingProduct.ProductName = productPrice?.Product?.Name;
                topSellingProduct.PriceName = productPrice?.PriceName;
                topSellingProduct.Quantity = productPriceItem.Quantity;
                topSellingProduct.TotalCost = productPriceItem.Quantity * productPrice.PriceValue;
                topSellingProduct.Thumbnail = productPrice?.Product?.Thumbnail;
                topSellingProduct.Category = productPrice?.Product?.ProductProductCategories.FirstOrDefault()?.ProductCategory?.Name;
                topSellingProduct.TotalProductCost = productCost?.ProductCost ?? 0;
                listTopSellingProduct.Add(topSellingProduct);
                no++;

            }

            var totalQuantity = listTopSellingProduct.Sum(x => x.Quantity);
            var totalAmount = listTopSellingProduct.Sum(x => x.TotalCost);
            var totalCost = listTopSellingProduct.Sum(x => x.TotalProductCost);

            if (request.SortNo == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.No).ToList();
                goto returnList;
            }
            else if (request.SortNo == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.No).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }

            if (request.SortProductName == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.ProductName).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
            else if (request.SortProductName == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.ProductName).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }

            if (request.SortCategory == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.Category).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
            else if (request.SortCategory == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.Category).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }

            if (request.SortQuantity == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.Quantity).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
            else if (request.SortQuantity == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.Quantity).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }

            if (request.SortAmount == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.TotalCost).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
            else if (request.SortAmount == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.TotalCost).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }

            if (request.SortCost == SortConstants.ASC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.TotalProductCost).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
            else if (request.SortCost == SortConstants.DESC)
            {
                listTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.TotalProductCost).Take(request.PageNumber * request.PageSize).ToList();
                goto returnList;
            }
        
            listTopSellingProduct = listTopSellingProduct.OrderBy(x => x.No).Take(request.PageNumber * request.PageSize).ToList();

            returnList:
            var response = new GetOrderSoldProductResponse()
            {
                ListSoldProduct = listTopSellingProduct,
                TotalQuantity = totalQuantity,
                TotalAmount = totalAmount,
                TotalCost = totalCost
            };

            return response;
        }
    }
}

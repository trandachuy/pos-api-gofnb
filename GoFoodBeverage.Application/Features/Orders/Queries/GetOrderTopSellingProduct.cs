using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderTopSellingProductRequest : IRequest<GetOrderTopSellingProductResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetOrderTopSellingProductResponse
    {
        public IEnumerable<OrderTopSellingProductModel> ListTopSellingProduct { get; set; }
        public IEnumerable<OrderTopCustomerModel> ListTopCustomer { get; set; }
        public IEnumerable<OrderTopSellingProductModel> ListWorstSellingProduct { get; set; }
    }

    public class GetOrderTopSellingProductHandler : IRequestHandler<GetOrderTopSellingProductRequest, GetOrderTopSellingProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetOrderTopSellingProductHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetOrderTopSellingProductResponse> Handle(GetOrderTopSellingProductRequest request, CancellationToken cancellationToken)
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
                                                                  .Include(x => x.Product)
                                                                  .Select(x => new { x.Id, x.PriceName, x.PriceValue, x.Product })
                                                                  .ToListAsync(cancellationToken: cancellationToken);
            var listTopSellingProduct = new List<OrderTopSellingProductModel>();
            var listProductIdsTemp = new List<Guid?>();

            foreach (var productPriceItem in listAllProductPricesGroup)
            {
                var topSellingProduct = new OrderTopSellingProductModel();
                var productPrice = listProductPrice.FirstOrDefault(x => x.Id == productPriceItem.ProductPriceId);
                if (!listProductIdsTemp.Contains(productPrice?.Product?.Id))
                {
                    listProductIdsTemp.Add(productPrice?.Product?.Id);
                }
                else
                {
                    continue;
                }
                topSellingProduct.ProductName = productPrice?.Product?.Name;
                topSellingProduct.PriceName = productPrice?.PriceName;
                topSellingProduct.Quantity = productPriceItem.Quantity;
                topSellingProduct.TotalCost = productPriceItem.Quantity * productPrice.PriceValue;
                topSellingProduct.Thumbnail = productPrice?.Product?.Thumbnail;
                listTopSellingProduct.Add(topSellingProduct);
            }
            var listOrderCustomerNotNull = listOrder.Where(x => x.CustomerId != null).ToList();
            var listCustomereObject = listOrderCustomerNotNull.GroupBy(x => new { x.CustomerId })
                                                              .Select(g => new { CustomerId = g.Key.CustomerId, Cost = g.Sum(x => x.PriceAfterDiscount) })
                                                              .OrderByDescending(x => x.Cost).Take(5).ToList();
            var listCustomerIds = listCustomereObject.Select(x => x.CustomerId);
            var listCustomer = await _unitOfWork.Customers.Find(x => x.StoreId == loggedUser.StoreId && listCustomerIds.Contains(x.Id))
                                                          .Select(x => new { x.Id, x.FullName, x.Thumbnail })
                                                          .ToListAsync(cancellationToken: cancellationToken);
            var listTopCustomer = new List<OrderTopCustomerModel>();
            foreach (var customerItem in listCustomereObject)
            {
                var topTopCustomer = new OrderTopCustomerModel();
                var customer = listCustomer.FirstOrDefault(x => x.Id == customerItem.CustomerId);
                topTopCustomer.Cost = customerItem.Cost;
                topTopCustomer.CustomerName = customer?.FullName;
                topTopCustomer.Thumbnail = customer?.Thumbnail;
                listTopCustomer.Add(topTopCustomer);
            }
            var response = new GetOrderTopSellingProductResponse()
            {
                ListTopSellingProduct = listTopSellingProduct.OrderByDescending(x => x.Quantity).ThenByDescending(x=>x.TotalCost).Take(5).Select((b, index) => new OrderTopSellingProductModel
                {
                   No = index + 1,
                   TotalCost = b.TotalCost,
                   PriceName = b.PriceName,
                   ProductName = b.ProductName,
                   Quantity = b.Quantity,
                   Thumbnail = b.Thumbnail
                }),
                ListTopCustomer = listTopCustomer.Select((b, index) =>new OrderTopCustomerModel {
                    No = index + 1,
                    Cost = b.Cost,
                    CustomerName = b.CustomerName,
                    Thumbnail = b.Thumbnail
                }),
                ListWorstSellingProduct = listTopSellingProduct.OrderBy(x=>x.Quantity).ThenBy(x => x.TotalCost).Take(5).Select((b, index) => new OrderTopSellingProductModel
                {
                    No = index + 1,
                    TotalCost = b.TotalCost,
                    PriceName = b.PriceName,
                    ProductName = b.ProductName,
                    Quantity = b.Quantity,
                    Thumbnail = b.Thumbnail
                })
            };

            return response;
        }
    }
}

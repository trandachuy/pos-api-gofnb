using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces.POS;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetOrderDetailByIdRequest : IRequest<GetOrderDetailByIdRResponse>
    {
        public Guid OrderId { get; set; }
    }

    public class GetOrderDetailByIdRResponse
    {
        public PosOrderDetailModel Order { get; set; }
    }

    public class GetOrderDetailByIdRHandler : IRequestHandler<GetOrderDetailByIdRequest, GetOrderDetailByIdRResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public GetOrderDetailByIdRHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IOrderService orderService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<GetOrderDetailByIdRResponse> Handle(GetOrderDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var orderDetail = new PosOrderDetailModel();
            var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                                   .AsNoTracking()
                                   .Include(x => x.OrderItems)
                                   .Include(x => x.OrderFees)
                                   .Include(x => x.AreaTable)
                                   .ThenInclude(x => x.Area)
                                   .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);

            var productPriceIds = order.OrderItems.Select(x => x.ProductPriceId.Value).ToList();
            var productPriceIdsDistinct = productPriceIds.Distinct();
            var orderItemIds = order.OrderItems.Select(x => x.Id);
            var orderItemToppings = _unitOfWork.OrderItemToppings.Find(x => x.StoreId == loggedUser.StoreId && orderItemIds.Contains(x.OrderItemId)).ToList();
            var orderItemOptions = _unitOfWork.OrderItemOptions.Find(x => x.StoreId == loggedUser.StoreId && orderItemIds.Contains(x.OrderItemId)).ToList();

            orderDetail.PosOrderDetailProducts = new List<PosOrderDetailProductModel>();
            foreach (var productPriceId in productPriceIdsDistinct)
            {
                var posOrderDetailProduct = new PosOrderDetailProductModel();
                var orderItem = order.OrderItems.FirstOrDefault(x => x.ProductPriceId == productPriceId && x.StatusId != EnumOrderItemStatus.Canceled);
                if (orderItem != null)
                {
                    posOrderDetailProduct.ProductName = orderItem?.ProductName;
                    posOrderDetailProduct.ProductPriceName = orderItem?.ProductPriceName;
                    posOrderDetailProduct.ProductPriceValue = orderItem.OriginalPrice;
                    posOrderDetailProduct.Quantity = productPriceIds.Count(x => x == productPriceId);

                    var listOrderItembyProductPriceId = order.OrderItems.Where(x => x.ProductPriceId == productPriceId);
                    var listOrderItemIds = listOrderItembyProductPriceId.Select(x => x.Id);
                    posOrderDetailProduct.ProductDetailOptions = orderItemOptions.Where(x => listOrderItemIds.Any(id => id == x.OrderItemId)).Select(x => new ProductDetailOption { OptionName = x.OptionName, OptionLevelName = x.OptionLevelName }).ToList();
                    posOrderDetailProduct.ProductDetailToppings = orderItemToppings.Where(x => listOrderItemIds.Any(id => id == x.OrderItemId)).Select(x => new ProductDetailTopping { ToppingName = x.ToppingName, ToppingValue = x.ToppingValue, Quantity = x.Quantity }).ToList();
                    posOrderDetailProduct.PromotionDiscountValue = listOrderItembyProductPriceId.FirstOrDefault()?.PromotionDiscountValue;
                    posOrderDetailProduct.IsPromotionDiscountPercentage = listOrderItembyProductPriceId.FirstOrDefault()?.IsPromotionDiscountPercentage;
                    posOrderDetailProduct.PromotionName = listOrderItembyProductPriceId.FirstOrDefault()?.PromotionName;
                    posOrderDetailProduct.TotalNotDiscount = posOrderDetailProduct.ProductPriceValue * posOrderDetailProduct.Quantity;
                    posOrderDetailProduct.Total = posOrderDetailProduct.IsPromotionDiscountPercentage == false ? (posOrderDetailProduct.TotalNotDiscount - posOrderDetailProduct.PromotionDiscountValue) : (posOrderDetailProduct.TotalNotDiscount * (100 - posOrderDetailProduct.PromotionDiscountValue)) / 100;
                    orderDetail.PosOrderDetailProducts.Add(posOrderDetailProduct);
                }
            }

            decimal totalAmount = 0;
            foreach (var item in orderDetail.PosOrderDetailProducts)
            {
                totalAmount = totalAmount + item.Total ?? 0;
            }

            //This logic will be updated after handle order completed status. Get backup data from RestoreData of Order
            orderDetail.OrderDetailCustomer = new OrderDetailCustomer
            {
                CustomerId = order.CustomerId
            };

            if (order.CustomerId != null && order.StatusId == EnumOrderStatus.Completed)
            {
                var customer = _unitOfWork.Customers.Find(x => x.StoreId == loggedUser.StoreId && x.Id == order.CustomerId).Include(x => x.CustomerPoint).FirstOrDefault();
                orderDetail.OrderDetailCustomer.CustomerName = customer?.FirstName + " " + customer?.LastName;
                orderDetail.OrderDetailCustomer.CustomerPhone = customer.PhoneNumber;
                var customerMemberships = await _unitOfWork.CustomerMemberships.Find(x => x.StoreId == loggedUser.StoreId.Value).ToListAsync();
                customerMemberships.Add(new CustomerMembershipLevel { AccumulatedPoint = 0 });
                customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
                foreach (var membership in customerMemberships)
                {
                    if (customer.CustomerPoint.AccumulatedPoint >= membership.AccumulatedPoint)
                    {
                        orderDetail.OrderDetailCustomer.CustomerRank = membership.Name;
                        orderDetail.OrderDetailCustomer.CustomerDiscount = membership.Discount;
                        var discountAmount = (totalAmount * (100 - membership.Discount)) / 100;
                        orderDetail.CustomerTotal = discountAmount > membership.MaximumDiscount ? membership.MaximumDiscount : discountAmount;
                        break;
                    }
                }
            }

            //This logic will be updated after handle order completed status. Get backup data from RestoreData of Order
            orderDetail.OrderDetailPromotion = new OrderDetailPromotion
            {
                Ispercent = order.IsPromotionDiscountPercentage,
                PromotionDiscountValue = order.PromotionDiscountValue,
                PromotionName = order.PromotionName
            };

            orderDetail.PromotionTotal = order.IsPromotionDiscountPercentage == false ? (totalAmount * (100 - order.PromotionDiscountValue)) / 100 : totalAmount - order.PromotionDiscountValue;

            //This logic will be updated after handle order completed status. Get backup data from RestoreData of Order
            orderDetail.OrderDetailFees = new List<OrderDetailFee>();
            orderDetail.FeeTotal = 0;
            foreach (var item in order.OrderFees)
            {
                var orderDetailFee = new OrderDetailFee
                {
                    FeeName = item.FeeName,
                    FeeDiscountValue = item.FeeValue,
                    Ispercent = item.IsPercentage
                };
                orderDetailFee.FeeAmount = orderDetailFee.Ispercent == false ? (totalAmount * orderDetailFee.FeeDiscountValue) / 100 : totalAmount - orderDetailFee.FeeDiscountValue;
                orderDetail.OrderDetailFees.Add(orderDetailFee);
                orderDetail.FeeTotal += orderDetailFee.FeeAmount;
            }

            orderDetail.Id = order.Id;
            orderDetail.OrderType = Enum.GetName(typeof(EnumOrderType), order.OrderTypeId);
            orderDetail.OrderTime = order.CreatedTime;
            orderDetail.OrderTotalItems = orderDetail.PosOrderDetailProducts.Count;
            orderDetail.Table = order?.AreaTable?.Name;
            orderDetail.Area = order?.AreaTable?.Area?.Name;
            orderDetail.OrderCode = order.StringCode;
            orderDetail.TotalAmount = totalAmount;

            return new GetOrderDetailByIdRResponse { Order = orderDetail };
        }
    }
}

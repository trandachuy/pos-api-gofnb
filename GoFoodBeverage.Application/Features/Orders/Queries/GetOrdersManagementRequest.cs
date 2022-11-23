using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Order;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrdersManagementRequest : IRequest<GetOrdersManagementResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetOrdersManagementResponse
    {
        public IEnumerable<OrderModel> Orders { get; set; }

        public OrderReportFilterModel OrderReportFilters { get; set; }

        public OrderTransactionReport OrderTransactionReport { get; set; }

        public int Total { get; set; }
    }

    public class GetOrdersManagementRequestHandler : IRequestHandler<GetOrdersManagementRequest, GetOrdersManagementResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetOrdersManagementRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetOrdersManagementResponse> Handle(GetOrdersManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime startDate = request.StartDate;
            DateTime endDate = request.EndDate.AddDays(1).AddSeconds(-1);
            var startDateCompare = DateTime.Today.AddDays(-1);
            var endDateCompare = DateTime.Today.AddSeconds(-1);

            switch (request.BusinessSummaryWidgetFilter)
            {
                case EnumBusinessSummaryWidgetFilter.Yesterday:
                case EnumBusinessSummaryWidgetFilter.Today:
                    startDateCompare = startDate.AddDays(-1);
                    endDateCompare = startDate.AddSeconds(-1);
                    break;
                case EnumBusinessSummaryWidgetFilter.ThisWeek:
                case EnumBusinessSummaryWidgetFilter.LastWeek:
                    startDateCompare = startDate.AddDays(-7);
                    endDateCompare = startDate.AddSeconds(-1);
                    break;
                case EnumBusinessSummaryWidgetFilter.ThisMonth:
                case EnumBusinessSummaryWidgetFilter.LastMonth:
                    startDateCompare = startDate.AddMonths(-1);
                    endDateCompare = startDate.AddSeconds(-1);
                    break;
                case EnumBusinessSummaryWidgetFilter.ThisYear:
                    startDateCompare = startDate.AddYears(-1);
                    endDateCompare = startDate.AddSeconds(-1);
                    break;
                default:
                    var days = (request.EndDate - request.StartDate).TotalDays;
                    startDateCompare = request.StartDate.AddDays(-(days + 1));
                    endDateCompare = request.StartDate.AddSeconds(-1);
                    break;
            }

            var listAllOrder = _unitOfWork.Orders
                .GetAllOrdersInStore(loggedUser.StoreId)
                .Include(o => o.Customer).ThenInclude(c => c.CustomerPoint)
                .AsNoTracking();

            if (request.BranchId.HasValue)
            {
                listAllOrder = listAllOrder.Where(o => o.BranchId == request.BranchId);
            }

            var listOrderCurrent = listAllOrder.Where(o => o.CreatedTime >= startDate && o.CreatedTime <= endDate).AsNoTracking();

            string keySearch = request?.KeySearch?.Trim().ToLower();

            if (!string.IsNullOrEmpty(keySearch))
            {
                int orderCode = -1;
                if (int.TryParse(keySearch, out orderCode))
                {
                    listOrderCurrent = listOrderCurrent
                        .Where(o => o.Code.Contains(orderCode.ToString()));
                }
                else
                {
                    string firstCharacterFromString = keySearch.Substring(0, 1)?.ToUpper();

                    if (keySearch.Length > 1)
                    {
                        string numberFromString = keySearch.Substring(1);
                        int.TryParse(numberFromString, out orderCode);
                    }

                    switch (firstCharacterFromString)
                    {
                        case "I":
                            listOrderCurrent = listOrderCurrent
                                .Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.Instore) : o.OrderTypeId == EnumOrderType.Instore);
                            break;
                        case "D":
                            listOrderCurrent = listOrderCurrent
                                .Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.Delivery) : o.OrderTypeId == EnumOrderType.Delivery);
                            break;
                        case "T":
                            listOrderCurrent = listOrderCurrent
                                .Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.TakeAway) : o.OrderTypeId == EnumOrderType.TakeAway);
                            break;
                        default:
                            break;
                    }
                }
            }

            //Filter
            var orderReportFilter = new OrderReportFilterModel();
            orderReportFilter.ServiceTypes = Enum.GetValues(typeof(EnumOrderType))
                                .Cast<EnumOrderType>()
                                .Select(e => new OrderReportFilterModel.ServiceTypeDto { Id = e })
                                .ToList();
            orderReportFilter.PaymentMethods = Enum.GetValues(typeof(EnumPaymentMethod))
                                .Cast<EnumPaymentMethod>()
                                .Select(e => new OrderReportFilterModel.PaymentMethodDto { Id = e })
                                .ToList();
            orderReportFilter.Customers = _unitOfWork.Customers.Find(c => c.StoreId == loggedUser.StoreId)
                .Select(c => new OrderReportFilterModel.OrderReportFilterCustomerDto { Id = c.Id, Name = c.FullName })
                .ToList();
            orderReportFilter.OrderStatus = Enum.GetValues(typeof(EnumOrderStatus))
                                .Cast<EnumOrderStatus>()
                                .Select(e => new OrderReportFilterModel.OrderStatusDto { Id = e })
                                .ToList();
            //Transaction Order Pie Chart

            var orderTransactionReport = new OrderTransactionReport();
            //Total Order
            orderTransactionReport.TotalOrderCurrent = listOrderCurrent.Count();
            var totalOrderPrev = listAllOrder.Where(o => o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderPercent, totalOrderIncrease) = CalculateUpDown(totalOrderPrev, orderTransactionReport.TotalOrderCurrent);
            orderTransactionReport.TotalOrderPercent = totalOrderPercent;
            orderTransactionReport.TotalOrderIncrease = totalOrderIncrease;

            //Total Order Cancel
            orderTransactionReport.TotalOrderCancelCurrent = listOrderCurrent.Where(x => x.StatusId == EnumOrderStatus.Canceled).Count();
            var totalOrderCancelPrev = listAllOrder.Where(o => o.StatusId == EnumOrderStatus.Canceled && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderCancelPercent, totalOrderCancelIncrease) = CalculateUpDown(totalOrderCancelPrev, orderTransactionReport.TotalOrderCancelCurrent);
            orderTransactionReport.TotalOrderCancelPercent = totalOrderCancelPercent;
            orderTransactionReport.TotalOrderCancelIncrease = totalOrderCancelIncrease;

            //Total Order Instore
            orderTransactionReport.TotalOrderInstoreCurrent = listOrderCurrent.Where(x => x.OrderTypeId == EnumOrderType.Instore).Count();
            var totalOrderInstorePrev = listAllOrder.Where(o => o.OrderTypeId == EnumOrderType.Instore && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderInstorePercent, totalOrderInstoreIncrease) = CalculateUpDown(totalOrderInstorePrev, orderTransactionReport.TotalOrderInstoreCurrent);
            orderTransactionReport.TotalOrderInstorePercent = totalOrderInstorePercent;
            orderTransactionReport.TotalOrderInstoreIncrease = totalOrderInstoreIncrease;

            //Total Order TakeAway
            orderTransactionReport.TotalOrderTakeAwayCurrent = listOrderCurrent.Where(x => x.OrderTypeId == EnumOrderType.TakeAway).Count();
            var totalOrderTakeAwayPrev = listAllOrder.Where(o => o.OrderTypeId == EnumOrderType.TakeAway && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderTakeAwayPercent, totalOrderTakeAwayIncrease) = CalculateUpDown(totalOrderTakeAwayPrev, orderTransactionReport.TotalOrderTakeAwayCurrent);
            orderTransactionReport.TotalOrderTakeAwayPercent = totalOrderTakeAwayPercent;
            orderTransactionReport.TotalOrderTakeAwayIncrease = totalOrderTakeAwayIncrease;

            //Total Order GoF&B App
            orderTransactionReport.TotalOrderGoFnBAppCurrent = listOrderCurrent.Where(x => x.PlatformId == EnumPlatform.GoFnBApp.ToGuid()).Count();
            var totalOrderGoFnBAppPrev = listAllOrder.Where(o => o.PlatformId == EnumPlatform.GoFnBApp.ToGuid() && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderGoFnBAppPercent, totalOrderGoFnBAppIncrease) = CalculateUpDown(totalOrderGoFnBAppPrev, orderTransactionReport.TotalOrderGoFnBAppCurrent);
            orderTransactionReport.TotalOrderGoFnBAppPercent = totalOrderGoFnBAppPercent;
            orderTransactionReport.TotalOrderGoFnBAppIncrease = totalOrderGoFnBAppIncrease;

            //Total Order Store Web
            orderTransactionReport.TotalOrderStoreWebCurrent = listOrderCurrent.Where(x => x.PlatformId == EnumPlatform.StoreWebsite.ToGuid()).Count();
            var totalOrderStoreWebPrev = listAllOrder.Where(o => o.PlatformId == EnumPlatform.StoreWebsite.ToGuid() && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderStoreWebPercent, totalOrderStoreWebIncrease) = CalculateUpDown(totalOrderGoFnBAppPrev, orderTransactionReport.TotalOrderStoreWebCurrent);
            orderTransactionReport.TotalOrderStoreWebPercent = totalOrderStoreWebPercent;
            orderTransactionReport.TotalOrderStoreWebIncrease = totalOrderStoreWebIncrease;

            //Total Order Store App
            orderTransactionReport.TotalOrderStoreAppCurrent = listOrderCurrent.Where(x => x.PlatformId == EnumPlatform.StoreMobileApp.ToGuid()).Count();
            var totalOrderStoreAppPrev = listAllOrder.Where(o => o.PlatformId == EnumPlatform.StoreMobileApp.ToGuid() && o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare).Count();
            var (totalOrderStoreAppPercent, totalOrderStoreAppIncrease) = CalculateUpDown(totalOrderStoreAppPrev, orderTransactionReport.TotalOrderStoreAppCurrent);
            orderTransactionReport.TotalOrderStoreAppPercent = totalOrderStoreAppPercent;
            orderTransactionReport.TotalOrderStoreAppIncrease = totalOrderStoreAppIncrease;

            var listOrderOrdered = listOrderCurrent.OrderByDescending(o => o.CreatedTime);
            var listOrderByPaging = await listOrderOrdered.ToPaginationAsync(request.PageNumber, request.PageSize);
            var listOrderModels = _mapper.Map<IEnumerable<OrderModel>>(listOrderByPaging.Result);

            //Customer
            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            listOrderModels.ToList().ForEach(order =>
            {
                foreach (var membership in customerMemberships)
                {
                    if (order?.Customer?.AccumulatedPoint >= membership?.AccumulatedPoint)
                    {
                        order.Customer.Rank = membership.Name;
                        break;
                    }
                }

            });

            var response = new GetOrdersManagementResponse()
            {
                Orders = listOrderModels,
                OrderReportFilters = orderReportFilter,
                OrderTransactionReport = orderTransactionReport,
                Total = listOrderByPaging.Total
            };

            return response;
        }

        public (double, bool) CalculateUpDown(double prev, double current)
        {
            if (prev == 0)
            {
                if (current == 0)
                {
                    return (0, true);
                }
                else
                {
                    return (100, true);
                }
            }
            else
            {
                var percent = Math.Round((100 * current) / (double)prev) - 100;
                if (percent == 0)
                {
                    return (Math.Abs(percent), true);
                }
                else
                {
                    return (Math.Abs(percent), percent >= 0);
                }
            }
        }
    }
}

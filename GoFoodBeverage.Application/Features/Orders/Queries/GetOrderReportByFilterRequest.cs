using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Order;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderReportByFilterRequest : IRequest<GetOrderReportByFilterResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public int? ServiceTypeId { get; set; }

        public int? PaymentMethodId { get; set; }

        public Guid? CustomerId { get; set; }

        public int? OrderStatusId { get; set; }
    }

    public class GetOrderReportByFilterResponse
    {
        public IEnumerable<OrderModel> Orders { get; set; }

        public int Total { get; set; }
    }

    public class GetOrderReportByFilterRequestHandler : IRequestHandler<GetOrderReportByFilterRequest, GetOrderReportByFilterResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetOrderReportByFilterRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetOrderReportByFilterResponse> Handle(GetOrderReportByFilterRequest request, CancellationToken cancellationToken)
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
                            listOrderCurrent = listOrderCurrent.Where(x => 1 == 0);
                            break;
                    }
                }
            }

            if (listOrderCurrent != null)
            {
                if (request.ServiceTypeId != null)
                {
                    listOrderCurrent = listOrderCurrent.Where(o => o.OrderTypeId == (EnumOrderType)request.ServiceTypeId);
                }

                if (request.PaymentMethodId != null)
                {
                    listOrderCurrent = listOrderCurrent.Where(o => o.PaymentMethodId == (EnumPaymentMethod)request.PaymentMethodId);
                }

                if (request.CustomerId != null)
                {
                    listOrderCurrent = listOrderCurrent.Where(o => o.CustomerId == request.CustomerId);
                }

                if (request.OrderStatusId != null)
                {
                    listOrderCurrent = listOrderCurrent.Where(o => o.StatusId == (EnumOrderStatus)request.OrderStatusId);
                }
            }

            var listOrderOrdered = listOrderCurrent.OrderByDescending(o => o.CreatedTime);
            var listOrderByPaging = await listOrderOrdered.ToPaginationAsync(request.PageNumber, request.PageSize);
            var listOrderModels = _mapper.Map<IEnumerable<OrderModel>>(listOrderByPaging.Result);

            var response = new GetOrderReportByFilterResponse()
            {
                Orders = listOrderModels,
                Total = listOrderByPaging.Total
            };

            return response;
        }
    }
}

using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderBusinessSummaryWidgetRequest : IRequest<GetOrderBusinessSummaryWidgetResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }
    }

    public class GetOrderBusinessSummaryWidgetResponse
    {
        public int TotalOrder { get; set; }

        public decimal PercentOrder { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal PercentRevenue { get; set; }

        public decimal TotalCost { get; set; }

        public decimal PercentCost { get; set; }
    }

    public class GetOrderBusinessSummaryWidgetRequestHandler : IRequestHandler<GetOrderBusinessSummaryWidgetRequest, GetOrderBusinessSummaryWidgetResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetOrderBusinessSummaryWidgetRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetOrderBusinessSummaryWidgetResponse> Handle(GetOrderBusinessSummaryWidgetRequest request, CancellationToken cancellationToken)
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

            var listOrder = _unitOfWork.Orders
                .GetAllOrdersInStore(loggedUser.StoreId)
                .AsNoTracking();
            if (request.BranchId.HasValue)
            {
                listOrder = listOrder.Where(o => o.BranchId == request.BranchId);
            }

            //Get orders yesterday
            var listOrderYesterDay = listOrder.Where(o => o.CreatedTime.Value.CompareTo(startDateCompare) >= 0 && endDateCompare.CompareTo(o.CreatedTime.Value) >= 0);

            //Get orders by filter (branchId, startDate, endDate)
            var listOrderByFilter = listOrder.Where(o => o.CreatedTime.Value.CompareTo(startDate) >= 0 && endDate.CompareTo(o.CreatedTime.Value) >= 0);

            // Total order yesterday
            var totalOrderYesterday = listOrderYesterDay.Count();
            // Total order by filter (branchId, startDate, endDate)
            var totalOrderByFilter = listOrderByFilter.Count();

            // Total revenue orders yesterday
            var totalRevenueYesterday = listOrderYesterDay.ToList().Sum(o => o.PriceAfterDiscount);
            // Total revenue orders filter (branchId, startDate, endDate)
            var totalRevenueByFilter = listOrderByFilter.ToList().Sum(o => o.PriceAfterDiscount);

            // Total cost orders yesterday
            var totalCostYesterday = listOrderYesterDay.ToList().Sum(o => o.TotalCost);
            // Total cost orders filter (branchId, startDate, endDate)
            var totalCostByFilter = listOrderByFilter.ToList().Sum(o => o.TotalCost);

            // Percentage between order yesterday and order by filter (branchId, startDate, endDate) 
            var percentOrder = totalOrderYesterday == 0 ? 0 : ((totalOrderByFilter - totalOrderYesterday) / totalOrderYesterday) * 100;
            var percentRevenue = totalRevenueYesterday == 0 ? 0 : ((totalRevenueByFilter - totalRevenueYesterday) / totalRevenueYesterday) * 100;
            var percentCost = totalCostYesterday == 0 ? 0 : ((totalCostByFilter - totalCostYesterday) / totalCostYesterday) * 100;

            var response = new GetOrderBusinessSummaryWidgetResponse()
            {
                TotalOrder = totalOrderByFilter,
                PercentOrder = percentOrder,
                TotalRevenue = totalRevenueByFilter,
                PercentRevenue = Math.Round(percentRevenue, 2),
                TotalCost = totalCostByFilter,
                PercentCost = Math.Round(percentCost, 2),
            };

            return response;
        }
    }
}

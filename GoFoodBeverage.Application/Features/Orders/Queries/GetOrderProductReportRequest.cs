using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Order;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderProductReportRequest : IRequest<GetOrderProductReportResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }
    }

    public class GetOrderProductReportResponse : ProductRevenueReportModel
    {
    }

    public class GetOrderProductReportHandler : IRequestHandler<GetOrderProductReportRequest, GetOrderProductReportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;

        public GetOrderProductReportHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IDateTimeService dateTimeService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _dateTimeService = dateTimeService;
        }

        public async Task<GetOrderProductReportResponse> Handle(GetOrderProductReportRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var startDate = request.StartDate;
            var endDate = request.EndDate.AddDays(1).AddSeconds(-1);
            var startDateCompare = _dateTimeService.NowUtc.AddDays(-1);
            var endDateCompare = _dateTimeService.NowUtc.AddSeconds(-1);

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

            var allOrders = _unitOfWork.Orders
                            .GetAllOrdersInStore(loggedUser.StoreId)
                            .AsNoTracking();

            if (request.BranchId.HasValue)
            {
                allOrders = allOrders.Where(o => o.BranchId == request.BranchId);
            }

            var currentOrders = allOrders.Where(o => o.CreatedTime >= startDate && o.CreatedTime <= endDate && o.StatusId != EnumOrderStatus.Canceled);
            var currentTotalSoldItems = await currentOrders
                                        .Include(o => o.OrderItems.Where(oi => oi.ProductPriceId.HasValue))
                                        .SelectMany(o => o.OrderItems)
                                        .SumAsync(oi => oi.Quantity, cancellationToken: cancellationToken);

            var previousOrders = allOrders.Where(o => o.CreatedTime >= startDateCompare && o.CreatedTime <= endDateCompare && o.StatusId != EnumOrderStatus.Canceled);
            var previousTotalSoldItems = await previousOrders
                                        .Include(o => o.OrderItems.Where(oi => oi.ProductPriceId.HasValue))
                                        .SelectMany(o => o.OrderItems)
                                        .SumAsync(oi => oi.Quantity, cancellationToken: cancellationToken);

            var currentTotalOrder = await currentOrders.CountAsync(cancellationToken: cancellationToken);
            var currentOrdersTotalRevenue = await currentOrders.SumAsync(o => o.OriginalPrice, cancellationToken: cancellationToken);
            var currentOrdersTotalCost = await currentOrders.SumAsync(o => o.TotalCost, cancellationToken: cancellationToken);
            var currentOrdersTotalProfit = currentOrdersTotalRevenue - currentOrdersTotalCost;

            var previousOrdersTotalRevenue = await previousOrders.SumAsync(o => o.OriginalPrice, cancellationToken: cancellationToken);
            var previousOrdersTotalCost = await previousOrders.SumAsync(o => o.TotalCost, cancellationToken: cancellationToken);
            var previousOrdersTotalProfit = previousOrdersTotalRevenue - previousOrdersTotalCost;

            var averageOrder = CalculateAverageOrder(currentTotalSoldItems, currentTotalOrder);
            var percentageIncreaseTotalSoldItem = CalculatePercentageIncrease(previousTotalSoldItems, currentTotalSoldItems);
            var percentageIncreaseTotalCost = CalculatePercentageIncrease(previousOrdersTotalCost, currentOrdersTotalCost);
            var percentageIncreaseProfit = CalculatePercentageIncrease(previousOrdersTotalProfit, currentOrdersTotalProfit);

            var response = new GetOrderProductReportResponse()
            {
                ProductCostReport = new ProductCostReportModel()
                {
                    TotalCost = currentOrdersTotalCost,
                    Percentage = percentageIncreaseTotalCost
                },
                OrderProductReport = new OrderProductReportModel() {
                    AverageOrder = averageOrder,
                    TotalOrder = currentTotalOrder,
                    TotalSoldItems = currentTotalSoldItems,
                    Percentage = percentageIncreaseTotalSoldItem,
                },
                TotalRevenue = currentOrdersTotalRevenue,
                TotalProfit = currentOrdersTotalProfit,
                ProfitPercentage = percentageIncreaseProfit
            };

            return response;
        }

        public static double CalculatePercentageIncrease(decimal prev, decimal current)
        {
            if (prev == 0)
            {
                return current == 0 ? 0 : 100;
            }
            else
            {
                var percentage = Math.Round((100 * current) / prev) - 100;

                return (double)percentage;
            }
        }

        public static double CalculateAverageOrder(double totalSoldItems, double totalItems)
        {
            var average = totalItems > 0 ? Math.Round((double)totalSoldItems / totalItems, 2) : 0;

            return average;
        }
    }
}

using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
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
    public class GetOrderBusinessRevenueWidgetRequest : IRequest<GetOrderBusinessRevenueWidgetResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }
    }

    public class GetOrderBusinessRevenueWidgetResponse
    {
        public decimal TotalCost { get; set; }

        public decimal PercentCost { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal PercentDiscount { get; set; }

        public decimal TotalShippingFee { get; set; }

        public decimal PercentShippingFee { get; set; }

        public decimal TotalExtraFee { get; set; }

        public decimal PercentExtraFee { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalRevenuePaid { get; set; }

        public decimal TotalRevenueUnpaid { get; set; }

        public decimal PercentRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public decimal TotalProfitPaid { get; set; }

        public decimal TotalProfitUnpaid { get; set; }

        public decimal PercentProfit { get; set; }
    }

    public class GetOrderBusinessRevenueWidgetRequestHandler : IRequestHandler<GetOrderBusinessRevenueWidgetRequest, GetOrderBusinessRevenueWidgetResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetOrderBusinessRevenueWidgetRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetOrderBusinessRevenueWidgetResponse> Handle(GetOrderBusinessRevenueWidgetRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime startDate = request.StartDate;
            DateTime endDate = request.EndDate;

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
                    var days = Math.Round((request.EndDate - request.StartDate).TotalDays);
                    startDateCompare = request.StartDate.AddDays(-(days));
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
            
            var listOrderPrevious = listOrder.Where(o => o.CreatedTime.Value.CompareTo(startDateCompare) >= 0 && endDateCompare.CompareTo(o.CreatedTime.Value) >= 0);
            var listOrderByFilter = listOrder.Where(o => o.CreatedTime.Value.CompareTo(startDate) >= 0 && endDate.CompareTo(o.CreatedTime.Value) >= 0);

            //Total Cost
            var totalCostPrevious = listOrderPrevious.Sum(x => x.TotalCost);
            var totalCostByFilter = listOrderByFilter.Sum(x => x.TotalCost);

            var percentCost = totalCostPrevious == 0 ? 0 : ((totalCostByFilter - totalCostPrevious) / totalCostPrevious) * 100;
            percentCost = Math.Round(percentCost, 2);

            var totalCostPaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid).Sum(x => x.TotalCost);
            var totalCostUnpaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid).Sum(x => x.TotalCost);

            //Total Discount
            var totalDiscountPrevious = listOrderPrevious.Sum(x => x.TotalDiscountAmount);
            var totalDiscountByFilter = listOrderByFilter.Sum(x => x.TotalDiscountAmount);

            var percentDiscount = totalDiscountPrevious == 0 ? 0 : ((totalDiscountByFilter - totalDiscountPrevious) / totalDiscountPrevious) * 100;
            percentDiscount = Math.Round(percentDiscount, 2);

            var totalDiscountPaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid).Sum(x => x.TotalDiscountAmount);
            var totalDiscountUnpaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid).Sum(x => x.TotalDiscountAmount);

            //Total Extra Fee
            List<Guid> oderPreviousIds = listOrderPrevious.Select(x => x.Id).ToList();
            List<Guid> oderByFilterIds = listOrderByFilter.Select(x => x.Id).ToList();

            var extraFeePrevious = await _unitOfWork.OrderFees
                .GetAll()
                .Where(x => x.StoreId == loggedUser.StoreId && oderPreviousIds.Contains(x.Id))
                .ToListAsync();

            var extraFeeByFilter = await _unitOfWork.OrderFees
                .GetAll()
                .Where(x => x.StoreId == loggedUser.StoreId && oderByFilterIds.Contains(x.Id))
                .ToListAsync();

            var totalExtraFeePrevious = extraFeePrevious.Sum(x => x.FeeValue);
            var totalExtraFeeByFilter = extraFeeByFilter.Sum(x => x.FeeValue);

            var percentExtraFee = totalExtraFeePrevious == 0 ? 0 : ((totalExtraFeeByFilter - totalExtraFeePrevious) / totalExtraFeePrevious) * 100;
            percentExtraFee = Math.Round(percentExtraFee, 2);

            var totalExtraFeePaid = _unitOfWork.OrderFees
                .GetAll()
                .Where(x => x.StoreId == loggedUser.StoreId && oderPreviousIds.Contains(x.Id) && x.Order.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid)
                .Sum(x => x.FeeValue);

            var totalExtraFeeUnpaid = _unitOfWork.OrderFees
                .GetAll()
                .Where(x => x.StoreId == loggedUser.StoreId && oderPreviousIds.Contains(x.Id) && x.Order.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid)
                .Sum(x => x.FeeValue);

            //Total Revenue
            var totalRevenuePrevious = listOrderPrevious.Sum(x => x.OriginalPrice);
            var totalRevenueByFilter = listOrderByFilter.Sum(x => x.OriginalPrice);

            var percentRevenue = totalRevenuePrevious == 0 ? 0 : ((totalRevenueByFilter - totalRevenuePrevious) / totalRevenuePrevious) * 100;
            percentRevenue = Math.Round(percentRevenue, 2);

            var totalRevenuePaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid).Sum(x => x.OriginalPrice);
            var totalRevenueUnpaid = listOrderByFilter.Where(x => x.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid).Sum(x => x.OriginalPrice);

            //Total Profit
            var totalProfitPrevious = totalRevenuePrevious - totalCostPrevious - totalDiscountPrevious;
            var totalProfitByFilter = totalRevenueByFilter - totalCostByFilter - totalDiscountByFilter;

            var percentProfit = totalRevenuePrevious == 0 ? 0 : ((totalProfitByFilter - totalProfitPrevious) / totalProfitPrevious) * 100;
            percentProfit = Math.Round(percentProfit, 2);

            var totalProfitPaid = totalRevenuePaid - totalCostPaid - totalDiscountPaid - totalExtraFeePaid;
            var totalProfitUnpain = totalRevenueUnpaid - totalCostUnpaid - totalDiscountUnpaid -totalExtraFeeUnpaid;

            var reponse = new GetOrderBusinessRevenueWidgetResponse
            {
                TotalCost = totalCostByFilter,
                PercentCost = percentCost,
                TotalDiscount = totalDiscountByFilter,
                PercentDiscount = percentDiscount,
                TotalShippingFee = 0,
                PercentShippingFee = 0,
                TotalExtraFee = totalExtraFeeByFilter,
                PercentExtraFee = percentExtraFee,
                TotalRevenue = totalRevenueByFilter,
                PercentRevenue = percentRevenue,
                TotalRevenuePaid = totalRevenuePaid,
                TotalRevenueUnpaid = totalRevenueUnpaid,
                TotalProfit = totalProfitByFilter,
                PercentProfit = percentProfit,
                TotalProfitPaid = totalProfitPaid,
                TotalProfitUnpaid = totalProfitUnpain
            };

            return reponse;
        }
    }
}

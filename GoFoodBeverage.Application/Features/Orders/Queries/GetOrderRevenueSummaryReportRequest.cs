using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderRevenueSummaryReportRequest : IRequest<GetOrderRevenueSummaryReportResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EnumBusinessSummaryWidgetFilter BusinessSummaryWidgetFilter { get; set; }
    }

    public class GetOrderRevenueSummaryReportResponse
    {
        public IEnumerable<RevenueByPlatformDto> RevenueByPlatforms { get; set; }

        public class RevenueByPlatformDto
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }

            public int TotalOrder { get; set; }

            public decimal TotalAmount { get; set; }
        }

        public IEnumerable<RevenueByPaymentMethodDto> RevenueByPaymentMethods { get; set; }

        public class RevenueByPaymentMethodDto
        {
            public EnumPaymentMethod Id { get; set; }

            public string Name { get; set; }

            public int TotalOrder { get; set; }

            public decimal TotalAmount { get; set; }
        }

        public IEnumerable<RevenueByServiceTypeDto> RevenueByServiceTypes { get; set; }

        public class RevenueByServiceTypeDto
        {
            public EnumOrderType Id { get; set; }

            public string Name { get; set; }

            public int TotalOrder { get; set; }

            public decimal TotalAmount { get; set; }
        }

        public IEnumerable<RevenueByOrderStatusDto> RevenueByOrderStatus { get; set; }

        public class RevenueByOrderStatusDto
        {
            public EnumOrderStatus Id { get; set; }

            public string Name { get; set; }

            public int TotalOrder { get; set; }

            public decimal TotalAmount { get; set; }
        }
    }

    public class GetRevenueByTypeResponseHandler : IRequestHandler<GetOrderRevenueSummaryReportRequest, GetOrderRevenueSummaryReportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetRevenueByTypeResponseHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetOrderRevenueSummaryReportResponse> Handle(GetOrderRevenueSummaryReportRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime startDate = request.StartDate.StartOfDay();
            DateTime endDate = request.EndDate.EndOfDay();

            var utcStartDate = startDate.ToUniversalTime();
            var utcEndDate = endDate.ToUniversalTime();

            var listOrder = await _unitOfWork.Orders
                .GetAllOrdersInStore(loggedUser.StoreId)
                .Include(o => o.Platform)
                .ToListAsync();
            if (request.BranchId.HasValue)
            {
                listOrder = listOrder.Where(o => o.BranchId == request.BranchId).ToList();
            }

            //Get orders by filter (branchId, startDate, endDate)
            var listOrderByFilter = listOrder
                .Where(o => o.CreatedTime.Value.CompareTo(utcStartDate) >= 0 && utcEndDate.CompareTo(o.CreatedTime.Value) >= 0)
                .ToList();

            //Get orders by platform
            var revenuesByPlatforms = listOrderByFilter
                .GroupBy(o => o.PlatformId)
                .Select(pf => new GetOrderRevenueSummaryReportResponse.RevenueByPlatformDto
                {
                    Id = pf.Key,
                    Name = pf.Where(o => o.PlatformId == pf.Key).Select(o => o.Platform?.Name).FirstOrDefault(),
                    TotalOrder = pf.Count(),
                    TotalAmount = pf.Sum(t => t.PriceAfterDiscount)
                })
                .ToList();

            var platforms = _unitOfWork.Platforms.GetAll().Where(p => p.StatusId == 1).ToList();

            platforms.ForEach(e =>
            {
                var existed = revenuesByPlatforms.FirstOrDefault(i => i.Id == e.Id);
                if (existed != null) return;

                var revenueByPlatform = new GetOrderRevenueSummaryReportResponse.RevenueByPlatformDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    TotalOrder = 0,
                    TotalAmount = 0
                };

                revenuesByPlatforms.Add(revenueByPlatform);
            });

            //Get orders by payment method
            var revenuesByPaymentMethods = listOrderByFilter
                .GroupBy(o => o.PaymentMethodId)
                .Select(p => new GetOrderRevenueSummaryReportResponse.RevenueByPaymentMethodDto
                {
                    Id = p.Key,
                    Name = p.Key.GetName(),
                    TotalOrder = p.Count(),
                    TotalAmount = p.Sum(t => t.PriceAfterDiscount)
                })
                .ToList();

            var paymentMethods = Enum.GetValues(typeof(EnumPaymentMethod))
                                .Cast<EnumPaymentMethod>()
                                .Select(e => e)
                                .ToList();

            paymentMethods.ForEach(e =>
            {
                var existed = revenuesByPaymentMethods.FirstOrDefault(i => i.Id == e);
                if (existed != null) return;

                var revenueByPaymentMethod = new GetOrderRevenueSummaryReportResponse.RevenueByPaymentMethodDto()
                {
                    Id = e,
                    Name = e.GetName(),
                    TotalOrder = 0,
                    TotalAmount = 0
                };

                revenuesByPaymentMethods.Add(revenueByPaymentMethod);
            });

            //Get orders by service type
            var revenuesByServiceTypes = listOrderByFilter
                .GroupBy(o => o.OrderTypeId)
                .Select(p => new GetOrderRevenueSummaryReportResponse.RevenueByServiceTypeDto
                {
                    Id = p.Key,
                    Name = p.Key.GetName(),
                    TotalOrder = p.Count(),
                    TotalAmount = p.Sum(t => t.PriceAfterDiscount)
                })
                .ToList();

            var serviceTypes = Enum.GetValues(typeof(EnumOrderType))
                                .Cast<EnumOrderType>()
                                .Select(e => e)
                                .ToList();

            serviceTypes.ForEach(e =>
            {
                var existed = revenuesByServiceTypes.FirstOrDefault(i => i.Id == e);
                if (existed != null) return;

                var revenueByServiceType = new GetOrderRevenueSummaryReportResponse.RevenueByServiceTypeDto()
                {
                    Id = e,
                    Name = e.GetName(),
                    TotalOrder = 0,
                    TotalAmount = 0
                };

                revenuesByServiceTypes.Add(revenueByServiceType);
            });

            //Get orders by order status
            var revenuesByOrderStatus = listOrderByFilter
                .GroupBy(o => o.StatusId)
                .Select(p => new GetOrderRevenueSummaryReportResponse.RevenueByOrderStatusDto
                {
                    Id = p.Key,
                    Name = p.Key.GetName(),
                    TotalOrder = p.Count(),
                    TotalAmount = p.Sum(t => t.PriceAfterDiscount)
                })
                .ToList();

            var orderStatus = Enum.GetValues(typeof(EnumOrderStatus))
                                .Cast<EnumOrderStatus>().Where(t => t != EnumOrderStatus.New)
                                .Select(e => e)
                                .ToList();

            orderStatus.ForEach(e =>
            {
                var existed = revenuesByOrderStatus.FirstOrDefault(i => i.Id == e);
                if (existed != null) return;

                var revenueByOrderStatus = new GetOrderRevenueSummaryReportResponse.RevenueByOrderStatusDto()
                {
                    Id = e,
                    Name = e.GetName(),
                    TotalOrder = 0,
                    TotalAmount = 0
                };

                revenuesByOrderStatus.Add(revenueByOrderStatus);
            });

            // Sort by total amount descending
            revenuesByOrderStatus = revenuesByOrderStatus.OrderByDescending(m => m.TotalAmount).ToList();

            var response = new GetOrderRevenueSummaryReportResponse()
            {
                RevenueByPlatforms = revenuesByPlatforms,
                RevenueByPaymentMethods = revenuesByPaymentMethods,
                RevenueByServiceTypes = revenuesByServiceTypes,
                RevenueByOrderStatus = revenuesByOrderStatus
            };

            return response;
        }
    }
}

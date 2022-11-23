using AutoMapper;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Report;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Report.Queries
{
    public class GetTopCustomerReportRequest : IRequest<GetTopCustomerReportResponse>
    {
        public string KeySearch { get; set; }

        public Guid? BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string SortCustomerName { get; set; }

        public string SortOrderNumber { get; set; }

        public string SortTotalAmount { get; set; }
    }

    public class GetTopCustomerReportResponse
    {
        public IEnumerable<TopCustomerReportModel> TopCustomerList { get; set; }

        public int TotalCustomer { get; set; }

        public decimal? TotalOrder { get; set; }

        public decimal? TotalAmount { get; set; }
    }

    public class GetTopCustomerReportRequestHandler : IRequestHandler<GetTopCustomerReportRequest, GetTopCustomerReportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetTopCustomerReportRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetTopCustomerReportResponse> Handle(GetTopCustomerReportRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime startDate = request.StartDate.StartOfDay().ToUniversalTime();
            DateTime endDate = request.EndDate.EndOfDay().ToUniversalTime();

            var customerList = await _unitOfWork.Customers
                                        .GetCustomerByKeySearchInStore(request.KeySearch, loggedUser.StoreId)
                                        .Where(o => o.CreatedTime.Value.CompareTo(startDate) >= 0
                                             && endDate.CompareTo(o.CreatedTime.Value) >= 0)
                                        .Include(x => x.CustomerPoint)
                                        .OrderBy(x => x.CreatedTime)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken: cancellationToken);

            var totalCustomer = customerList.Count;

            if (request.BranchId.HasValue)
            {
                customerList = customerList.Where(o => o.BranchId == request.BranchId).ToList();
            }

            var topCustomerList = _mapper.Map<List<TopCustomerReportModel>>(customerList);

            var customerPoint = customerList.Where(x => x.CustomerPoint != null).Select(x => x.CustomerPoint);

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId)
                .Select(x => new
                {
                    x.Name,
                    x.Color,
                    x.AccumulatedPoint
                })
                .OrderByDescending(x => x.AccumulatedPoint)
                .ToListAsync(cancellationToken);

            var customerListIds = topCustomerList.Select(x => x.Id).ToList();

            var orders = await _unitOfWork.Orders
                .Find(o => o.StoreId == loggedUser.StoreId
                        && o.CustomerId != null && customerListIds.Contains(o.CustomerId)
                        && o.StatusId != EnumOrderStatus.Canceled && o.StatusId != EnumOrderStatus.Draft)
                .Select(o => new
                {
                    o.Id,
                    o.CustomerId,
                    TotalAmount = o.PriceAfterDiscount + o.TotalFee + o.TotalTax + o.DeliveryFee - o.CustomerDiscountAmount,
                })
                .ToListAsync(cancellationToken);

            foreach (var customer in topCustomerList)
            {
                var customerOrder = orders.Where(o => o.CustomerId == customer.Id).ToList();

                customer.No = topCustomerList.IndexOf(customer) + 1;
                customer.Point = customerPoint.FirstOrDefault(x => x.CustomerId == customer.Id)?.AccumulatedPoint;
                customer.OrderNumber = customerOrder.Count;
                customer.TotalAmount = customerOrder.Sum(o => o.TotalAmount);

                foreach (var membership in customerMemberships)
                {
                    if (customer.Point >= membership.AccumulatedPoint)
                    {
                        customer.Rank = membership.Name;
                        customer.Color = membership.Color;
                        break;
                    }
                }
            }

            var totalOrder = topCustomerList.Sum(x => x.OrderNumber);
            var totalAmount = topCustomerList.Sum(x => x.TotalAmount);

            var skipPreviousItems = (request.PageNumber - 1) * request.PageSize;
            var pageSize = request.PageSize;

            /// Sort by customer name
            if (request.SortCustomerName == SortConstants.ASC)
            {
                topCustomerList = topCustomerList.OrderBy(x => x.FullName).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }
            else if (request.SortCustomerName == SortConstants.DESC)
            {
                topCustomerList = topCustomerList.OrderByDescending(x => x.FullName).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }

            /// Sort by order number
            if (request.SortOrderNumber == SortConstants.ASC)
            {
                topCustomerList = topCustomerList.OrderBy(x => x.OrderNumber).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }
            else if (request.SortOrderNumber == SortConstants.DESC)
            {
                topCustomerList = topCustomerList.OrderByDescending(x => x.OrderNumber).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }

            /// Sort by total amount
            if (request.SortTotalAmount == SortConstants.ASC)
            {
                topCustomerList = topCustomerList.OrderBy(x => x.TotalAmount).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }
            else if (request.SortTotalAmount == SortConstants.DESC)
            {
                topCustomerList = topCustomerList.OrderByDescending(x => x.TotalAmount).Skip(skipPreviousItems).Take(pageSize).ToList();
                goto returnList;
            }

            topCustomerList = topCustomerList.Skip(skipPreviousItems).Take(pageSize).ToList();

        returnList:
            var response = new GetTopCustomerReportResponse()
            {
                TopCustomerList = topCustomerList,
                TotalAmount = totalAmount,
                TotalOrder = totalOrder,
                TotalCustomer = totalCustomer
            };

            return response;
        }
    }
}

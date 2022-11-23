using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerReportWithPlatformRequest : IRequest<GetCustomerReportWithPlatformResponse>
    {
        public Guid? BranchId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public EnumSegmentTime SegmentTimeOption { get; set; }

        public DateTime? FromDateOfThePreviousSession { get; set; }

        public DateTime? ToDateOfThePreviousSession { get; set; }
    }

    public class GetCustomerReportWithPlatformResponse
    {
        public int TotalCustomer { get; set; }

        public int TotalOrder { get; set; }

        public decimal RevenueByCustomer { get; set; }

        public decimal AverageOrder { get; set; }

        public bool IsDecreaseCustomerFromThePreviousSession { get; set; }

        public decimal PercentageCustomerChangeFromThePreviousSession { get; set; }

        public List<PlatformStatistical> PlatformStatisticals { get; set; }

        public class PlatformStatistical
        {
            public Guid PlatformId { get; set; }

            public string PlatformName { get; set; }

            public int TotalCustomer { get; set; }

            public bool IsDecreaseFromThePreviousSession { get; set; }

            public decimal PercentageChangeFromThePreviousSession { get; set; }
        }
    }

    public class GetCustomerReportByPlatformRequestHandler : IRequestHandler<GetCustomerReportWithPlatformRequest, GetCustomerReportWithPlatformResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        private readonly MapperConfiguration _mapperConfiguration;

        public GetCustomerReportByPlatformRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetCustomerReportWithPlatformResponse> Handle(GetCustomerReportWithPlatformRequest request, CancellationToken cancellationToken)
        {
            // Get user information from the token.
            var loggedUser = await _userProvider.ProvideAsync();
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            HandlePeriodValue(request);

            // Get Platform List Active
            List<PlatFormReportForCustomerModel> platformList = await _unitOfWork.Platforms
                .GetAll()
                .AsNoTracking()
                .ProjectTo<PlatFormReportForCustomerModel>(_mapperConfiguration)
                .ToListAsync();

            // Get customer list
            List<CustomerReportByPlatformModel> customerListModel = await _unitOfWork.Customers
                .Find(customer => customer.StoreId == loggedUser.StoreId && customer.CreatedTime >= request.FromDateOfThePreviousSession && customer.CreatedTime <= request.ToDate)
                .Include(customer => customer.Platform)
                .AsNoTracking()
                .ProjectTo<CustomerReportByPlatformModel>(_mapperConfiguration)
                .ToListAsync();

            // Get order list
            List<OrderReportForCustomerModel> orderListModel = await _unitOfWork.Orders
                .Find(order => order.StoreId == loggedUser.StoreId && order.CreatedTime >= request.FromDateOfThePreviousSession && order.CreatedTime <= request.ToDate)
                .Include(order => order.Platform)
                .AsNoTracking()
                .ProjectTo<OrderReportForCustomerModel>(_mapperConfiguration)
                .ToListAsync();

            if (request.BranchId.HasValue)
            {
                customerListModel = customerListModel.Where(customer => customer.BranchId == request.BranchId).ToList();
                orderListModel = orderListModel.Where(order => order.BranchId == request.BranchId).ToList();
            }

            GetCustomerReportWithPlatformResponse response = new GetCustomerReportWithPlatformResponse();
            // Get customer list for current session
            List<CustomerReportByPlatformModel> customerListForCurrentSessionModel = customerListModel
                .Where(customer => customer.CreatedTime >= request.FromDate && customer.CreatedTime <= request.ToDate).ToList();
            // Get customer list for previous session
            List<CustomerReportByPlatformModel> customerListForPreviousSessionModel = customerListModel
                .Where(customer => customer.CreatedTime >= request.FromDateOfThePreviousSession && customer.CreatedTime <= request.ToDateOfThePreviousSession).ToList();
            // Get order list for current session
            List<OrderReportForCustomerModel> orderListForCurrentSessionModel = orderListModel
                .Where(customer => customer.CreatedTime >= request.FromDate && customer.CreatedTime <= request.ToDate).ToList();
            // Get order list for Previous session
            List<OrderReportForCustomerModel> orderListForPreviousSessionModel = orderListModel
                .Where(customer => customer.CreatedTime >= request.FromDateOfThePreviousSession && customer.CreatedTime <= request.ToDateOfThePreviousSession).ToList();

            response.PlatformStatisticals = new List<GetCustomerReportWithPlatformResponse.PlatformStatistical>();
            foreach (var platform in platformList)
            {
                var platformStatisticalItem = GetPlatformStatisticalItem(customerListForCurrentSessionModel, customerListForPreviousSessionModel, platform, null);
                response.PlatformStatisticals.Add(platformStatisticalItem);
            }

            response.TotalCustomer = customerListForCurrentSessionModel.Count();
            response.TotalOrder = orderListForCurrentSessionModel.Count();
            int numberOfCustomersChange = response.TotalCustomer - customerListForPreviousSessionModel.Count();
            response.IsDecreaseCustomerFromThePreviousSession = numberOfCustomersChange < 0 ? true : false;
            if (numberOfCustomersChange > 0 && customerListForPreviousSessionModel.Count() <= 0)
            {
                response.PercentageCustomerChangeFromThePreviousSession = numberOfCustomersChange * 100;
            }
            else if (customerListForPreviousSessionModel.Count() > 0)
            {
                decimal divideCustomerValue = Decimal.Divide(Math.Abs(numberOfCustomersChange), customerListForPreviousSessionModel.Count());
                response.PercentageCustomerChangeFromThePreviousSession = divideCustomerValue * 100;
            }
            else
            {
                response.PercentageCustomerChangeFromThePreviousSession = 0;
            }
            response.RevenueByCustomer = orderListForCurrentSessionModel.Sum(order => order.OriginalPrice - order.TotalDiscountAmount);
            // get information for platform other
            var platformStatisticalsOrderBy = response.PlatformStatisticals.OrderByDescending(a => a.TotalCustomer).ToList();
            List<Guid?> platformIds = platformStatisticalsOrderBy.Skip(4).Take(platformStatisticalsOrderBy.Count()).Select(a => (Guid?)a.PlatformId).ToList();
            response.PlatformStatisticals = platformStatisticalsOrderBy.Skip(0).Take(4).ToList();

            // Hanlde for platform other
            var platformStatisticalItemForOther = GetPlatformStatisticalItem(customerListForCurrentSessionModel, customerListForPreviousSessionModel,
                new PlatFormReportForCustomerModel() { Id = Guid.Empty, Name = "Other" }, platformIds);
            response.PlatformStatisticals.Add(platformStatisticalItemForOther);

            if (customerListForCurrentSessionModel.Count() > 0)
            {
                response.AverageOrder = orderListForCurrentSessionModel.Count() / customerListForCurrentSessionModel.Count();
            }

            return response;
        }

        private void HandlePeriodValue(GetCustomerReportWithPlatformRequest request)
        {
            request.FromDate = request.FromDate.StartOfDay().ToUniversalTime();
            request.ToDate = request.ToDate.EndOfDay().ToUniversalTime();
            var numOfDay = request.ToDate.Subtract(request.FromDate);

            switch (request.SegmentTimeOption)
            {
                case (EnumSegmentTime.Today):
                case (EnumSegmentTime.Yesterday):
                    request.FromDateOfThePreviousSession = request.FromDate.AddDays(-1);
                    request.ToDateOfThePreviousSession = request.ToDate.AddDays(-1);
                    break;
                case (EnumSegmentTime.ThisWeek):
                case (EnumSegmentTime.LastWeek):
                    request.FromDateOfThePreviousSession = request.FromDate.AddDays(-7);
                    request.ToDateOfThePreviousSession = request.FromDate.AddSeconds(-1);
                    break;
                case (EnumSegmentTime.ThisMonth):
                case (EnumSegmentTime.LastMonth):
                    request.FromDateOfThePreviousSession = request.FromDate.AddMonths(-1);
                    request.ToDateOfThePreviousSession = request.FromDate.AddSeconds(-1);
                    break;
                case (EnumSegmentTime.ThisYear):
                    request.FromDateOfThePreviousSession = request.FromDate.AddYears(-1);
                    request.ToDateOfThePreviousSession = request.FromDate.AddSeconds(-1);
                    break;
                default:
                    request.FromDateOfThePreviousSession = request.FromDate.AddDays(-numOfDay.TotalDays);
                    request.ToDateOfThePreviousSession = request.FromDate.AddSeconds(-1);
                    break;
            }
        }

        private GetCustomerReportWithPlatformResponse.PlatformStatistical GetPlatformStatisticalItem(
            List<CustomerReportByPlatformModel> customerListForCurrentSessionModel,
            List<CustomerReportByPlatformModel> customerListForPreviousSessionModel,
            PlatFormReportForCustomerModel platform,
            List<Guid?> platformList
        )
        {
            GetCustomerReportWithPlatformResponse.PlatformStatistical platformStatisticalItem = new GetCustomerReportWithPlatformResponse.PlatformStatistical();
            List<CustomerReportByPlatformModel> customerListForCurrentSessionByPlatformModel = new List<CustomerReportByPlatformModel>();
            List<CustomerReportByPlatformModel> customerListForPreviousSessionByPlatformModel = new List<CustomerReportByPlatformModel>();
            if (platformList != null && platformList.Count() > 0)
            {
                customerListForCurrentSessionByPlatformModel = customerListForCurrentSessionModel.Where(customer => platformList.Contains(customer.PlatformId) || customer.PlatformId == null).ToList();
                customerListForPreviousSessionByPlatformModel = customerListForPreviousSessionModel.Where(customer => platformList.Contains(customer.PlatformId) || customer.PlatformId == null).ToList();
            }
            else
            {
                customerListForCurrentSessionByPlatformModel = customerListForCurrentSessionModel.Where(customer => customer.PlatformId == platform.Id).ToList();
                customerListForPreviousSessionByPlatformModel = customerListForPreviousSessionModel.Where(customer => customer.PlatformId == platform.Id).ToList();
            }

            platformStatisticalItem.PlatformId = platform.Id;
            platformStatisticalItem.PlatformName = platform.Name;
            platformStatisticalItem.TotalCustomer = customerListForCurrentSessionByPlatformModel.Count();
            int numberOfCustomersChangeByPlatform = Math.Abs(customerListForCurrentSessionByPlatformModel.Count() - customerListForPreviousSessionByPlatformModel.Count());
            platformStatisticalItem.IsDecreaseFromThePreviousSession = numberOfCustomersChangeByPlatform < 0 ? true : false;            
            if (customerListForPreviousSessionByPlatformModel.Count() <= 0 && numberOfCustomersChangeByPlatform > 0)
            {
                platformStatisticalItem.PercentageChangeFromThePreviousSession = numberOfCustomersChangeByPlatform * 100;
            }
            else if (customerListForPreviousSessionByPlatformModel.Count() > 0)
            {
                decimal divideCustomerValue = Decimal.Divide(Math.Abs(numberOfCustomersChangeByPlatform), customerListForPreviousSessionModel.Count());
                platformStatisticalItem.PercentageChangeFromThePreviousSession = divideCustomerValue * 100;
            }
            else
            {
                platformStatisticalItem.PercentageChangeFromThePreviousSession = 0;
            }

            return platformStatisticalItem;
        }
    }
}

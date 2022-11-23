using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Order;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerOrderListRequest : IRequest<GetCustomerOrderListResponse>
    {
        public bool TakeTwoOrders { get; set; }

        public bool? SortByStoreName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Keyword { get; set; }

        public List<EnumOrderStatus> OrderStatusList { get; set; }
    }

    public class GetCustomerOrderListResponse
    {
        public List<BaseOrderModel> OrderList { get; set; }
    }

    public class GetCustomerOrderListRequestHandler : IRequestHandler<GetCustomerOrderListRequest, GetCustomerOrderListResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        private readonly MapperConfiguration _mapperConfiguration;

        public GetCustomerOrderListRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        /// <summary>
        /// This method is used to handle the data from the HTTP request.
        /// </summary>
        /// <param name="request">Data is mapped from the HTTP request.</param>
        /// <param name="cancellationToken">The current thread.</param>
        /// <returns></returns>
        public async Task<GetCustomerOrderListResponse> Handle(GetCustomerOrderListRequest request, CancellationToken cancellationToken)
        {
            // Get user information from the token.
            var loggedUser = _userProvider.GetLoggedCustomer();

            // Get user's orders by user id.
            var orders = _unitOfWork.Orders.GetOrderListByCustomerId(loggedUser.Id);

            if (request.OrderStatusList?.Count > 0)
            {
                orders = orders.
                    Where(order => request.OrderStatusList.Contains(order.StatusId));
            }

            // This line is used to take 2 orders and display on the modal when the users want to delete their account.
            if (request.TakeTwoOrders)
            {
                orders = orders.Take(2);
            }

            // Get orders by the start date and the end date.
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                orders = orders.
                    Where(order =>
                        order.CreatedTime >= request.StartDate &&
                        order.CreatedTime <= request.EndDate
                        );
            }

            // Search orders when the user types on the search box.
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                orders = orders.Where(order => order.Store.Title.Contains(request.Keyword) || order.Code.Contains(request.Keyword));
            }

            if (request.SortByStoreName.HasValue)
            {
                if (request.SortByStoreName.Value)
                {
                    orders = orders.OrderBy(x => x.Store.Title);
                }
                else
                {
                    orders = orders.OrderByDescending(x => x.CreatedTime);
                }
            }

            // Call
            var orderListModel = await orders.
                Include(order => order.Store).
                ProjectTo<BaseOrderModel>(_mapperConfiguration).
                ToListAsync();

            var dataToResponse = new GetCustomerOrderListResponse();
            dataToResponse.OrderList = orderListModel;

            return dataToResponse;
        }
    }
}

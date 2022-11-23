using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerByIdRequest : IRequest<GetCustomerByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerByIdResponse
    {
        public CustomerEditModel Customer { get; set; }
    }

    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdRequest, GetCustomerByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetCustomerByIdHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetCustomerByIdResponse> Handle(GetCustomerByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var customerData = await _unitOfWork.Customers
                .GetCustomerByKeySearchInStore(null, loggedUser.StoreId)
                .Include(x => x.Address)
                .Include(x => x.CustomerPoint)
                .Include(item => item.Platform)
                .Include(item => item.Account)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            var customerPoint = customerData?.CustomerPoint;
            var account = customerData?.Account;
            var customer = _mapper.Map<CustomerEditModel>(customerData);

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            foreach (var membership in customerMemberships)
            {
                if (customerPoint?.AccumulatedPoint >= membership.AccumulatedPoint)
                {
                    customer.Rank = membership.Name;
                    customer.BadgeColor = membership.Color;
                    break;
                }
            }

            customer.CustomerAddress = new CustomerAddress();
            if (customerData.Address != null)
            {
                customer.CustomerAddress.CityId = customerData.Address?.CityId;
                customer.CustomerAddress.DistrictId = customerData.Address?.DistrictId;
                customer.CustomerAddress.WardId = customerData.Address?.WardId;
                customer.CustomerAddress.Address1 = customerData.Address?.Address1;
                customer.CustomerAddress.CountryId = customerData.Address?.CountryId;
                customer.CustomerAddress.StateId = customerData.Address?.StateId;
                customer.CustomerAddress.CityTown = customerData.Address?.CityTown;
                customer.CustomerAddress.Address2 = customerData.Address?.Address2;
            } else
            {
                customer.CustomerAddress.CountryId = account.CountryId;
            }

            var listOrders = await _unitOfWork.Orders.Find(x => x.CustomerId == request.Id && x.StatusId != EnumOrderStatus.Canceled && x.StatusId != EnumOrderStatus.Draft).Select(x => new { x.PriceAfterDiscount, x.TotalFee, x.TotalTax, x.DeliveryFee }).ToListAsync();
            customer.TotalMoney = listOrders.Sum(x => x.PriceAfterDiscount) + listOrders.Sum(x => x.TotalFee) + listOrders.Sum(x => x.TotalTax) + listOrders.Sum(x => x.DeliveryFee);
            customer.TotalOrder = listOrders.Count();

            var response = new GetCustomerByIdResponse()
            {
                Customer = customer
            };

            return response;
        }
    }
}

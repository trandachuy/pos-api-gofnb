using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Customer.Queries
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
                .Include(s => s.Address).ThenInclude(a => a.Country)
                .Include(s => s.Address).ThenInclude(a => a.City)
                .Include(s => s.Address).ThenInclude(a => a.District)
                .Include(s => s.Address).ThenInclude(a => a.Ward)
                .Include(s => s.Address).ThenInclude(a => a.State)
                .Include(x => x.CustomerPoint)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            var customerPoint = customerData?.CustomerPoint;

            var customer = _mapper.Map<CustomerEditModel>(customerData);

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            customerMemberships.Add(new Domain.Entities.CustomerMembershipLevel { AccumulatedPoint = 0 });
            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            foreach (var membership in customerMemberships)
            {
                if (customerPoint?.AccumulatedPoint >= membership.AccumulatedPoint)
                {
                    customer.Rank = membership.Name;
                    break;
                }
            }

            var listOrders = await _unitOfWork.Orders.Find(x => x.CustomerId == request.Id && x.StatusId != EnumOrderStatus.Canceled && x.StatusId != EnumOrderStatus.Draft).Select(x => new { x.PriceAfterDiscount, x.TotalFee, x.TotalTax, x.DeliveryFee, x.CustomerDiscountAmount }).ToListAsync();
            customer.TotalMoney = listOrders.Sum(x => x.PriceAfterDiscount) + listOrders.Sum(x => x.TotalFee) + listOrders.Sum(x => x.TotalTax) + listOrders.Sum(x => x.DeliveryFee) - listOrders.Sum(x => x.CustomerDiscountAmount);
            customer.TotalOrder = listOrders.Count();

            var response = new GetCustomerByIdResponse()
            {
                Customer = customer
            };

            return response;
        }
    }
}

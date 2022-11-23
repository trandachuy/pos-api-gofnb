using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Fee;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.POS.Application.Features.Customer.Queries
{
    public class GetPosCustomersRequest : IRequest<GetPosCustomersResponse>
    {
        public string KeySearch { get; set; }
    }

    public class GetPosCustomersResponse
    {
        public IEnumerable<CustomerForOrderDetailModel> Customer { get; set; }
    }

    public class GetPosCustomersHandler : IRequestHandler<GetPosCustomersRequest, GetPosCustomersResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _iDateTimeService;

        public GetPosCustomersHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper, MapperConfiguration mapperConfiguration, IDateTimeService iDateTimeService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
            _iDateTimeService = iDateTimeService;
        }

        public async Task<GetPosCustomersResponse> Handle(GetPosCustomersRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var reponse = new List<CustomerForOrderDetailModel>();
            var customers = new List<Domain.Entities.Customer>();
            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync(cancellationToken: cancellationToken);
            
            customerMemberships.Add(new CustomerMembershipLevel { AccumulatedPoint = 0 });
            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            if (!string.IsNullOrEmpty(request.KeySearch))
            {
                 customers = _unitOfWork.Customers.Find(c =>c.StoreId == loggedUser.StoreId.Value && (c.FullName.ToLower().Contains(request.KeySearch.ToLower()) || c.PhoneNumber.ToLower().Contains(request.KeySearch.ToLower()))).OrderByDescending(x => x.CreatedTime).Include(x => x.CustomerPoint).ToList();
            }
            else
            {
                 customers = _unitOfWork.Customers.Find(c => c.StoreId == loggedUser.StoreId.Value).OrderByDescending(x => x.CreatedTime).Include(x => x.CustomerPoint).ToList();
            }

            foreach (var customer in customers)
            {
                var customerModel = new CustomerForOrderDetailModel();
                customerModel.Id = customer.Id;
                customerModel.CustomerPhone = customer.PhoneNumber;
                customerModel.CustomerName = customer.FullName;
                customerModel.Thumbnail = customer.Thumbnail;
                foreach (var membership in customerMemberships)
                {
                
                    if (customer.CustomerPoint != null && customer.CustomerPoint?.AccumulatedPoint >= membership.AccumulatedPoint)
                    {
                        customerModel.MemberShip = membership.Name;
                        customerModel.Discount = membership.Discount;
                        customerModel.MaximumDiscount = membership.MaximumDiscount;
                        break;
                    }
                }
                reponse.Add(customerModel);
            }

            return new GetPosCustomersResponse { Customer = reponse };
        }
    }
}

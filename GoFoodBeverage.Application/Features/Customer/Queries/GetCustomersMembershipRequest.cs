using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Customer;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomersMembershipRequest : IRequest<GetCustomersMembershipResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetCustomersMembershipResponse
    {
        public IEnumerable<CustomerMembershipModel> CustomerMemberships { get; set; }

        public int Total { get; set; }
    }

    public class GetCustomersMembershipHandler : IRequestHandler<GetCustomersMembershipRequest, GetCustomersMembershipResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetCustomersMembershipHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetCustomersMembershipResponse> Handle(GetCustomersMembershipRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var customerMembershipAll = _unitOfWork.CustomerMemberships
                .GetAllCustomerMembershipInStore(loggedUser.StoreId)
                .OrderBy(x => x.AccumulatedPoint);

            var checkExistClassicMember = customerMembershipAll.
                FirstOrDefault(x => x.AccumulatedPoint == 0 && x.Name == DefaultConstants.CLASSIC_MEMBER);

            if (checkExistClassicMember == null && customerMembershipAll.Count() == 0)
            {
                var customerMembershipModelAdd = new CustomerMembershipLevel { AccumulatedPoint = 0, Name = DefaultConstants.CLASSIC_MEMBER, StoreId = loggedUser.StoreId };
                await _unitOfWork.CustomerMemberships.AddAsync(customerMembershipModelAdd);
            }

            var customerMemberships = new List<CustomerMembershipModel>();
            var customerMembershipsData = _mapper.Map<List<CustomerMembershipModel>>(customerMembershipAll);
            customerMemberships.AddRange(customerMembershipsData);

            var lenCustomerMemberships = customerMemberships.Count;
            var index = 0;
            customerMemberships = customerMemberships.OrderBy(x => x.AccumulatedPoint).ToList();

            foreach (var item in customerMemberships)
            {
                int nextIndex = index + 1;
                item.No = customerMemberships.IndexOf(item) + ((request.PageNumber - 1) * request.PageSize) + 1;
                if (lenCustomerMemberships > 1 && nextIndex < lenCustomerMemberships)
                {
                    var listCustomerPoints = await _unitOfWork.
                    CustomerPoints.
                    CountCustomerPointByStoreIdAsync(
                        loggedUser.StoreId,
                        item.AccumulatedPoint,
                        customerMemberships[nextIndex].AccumulatedPoint
                    );

                    item.Member = listCustomerPoints;
                }
                else
                {
                    var listCustomerPoints = await _unitOfWork.
                     CustomerPoints.
                     CountCustomerPointByStoreIdAsync(
                         loggedUser.StoreId,
                         item.AccumulatedPoint,
                         0
                     );

                    item.Member = listCustomerPoints;
                }
                index++;
            }

            customerMemberships = customerMemberships.ToPagination(request.PageNumber, request.PageSize).Result.ToList();
            var response = new GetCustomersMembershipResponse()
            {
                CustomerMemberships = customerMemberships,
                Total = customerMembershipAll.Count() + 1
            };

            return response;
        }
    }
}

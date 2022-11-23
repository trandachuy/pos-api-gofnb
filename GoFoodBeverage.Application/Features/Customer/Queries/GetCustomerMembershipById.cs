using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerMembershipByIdRequest : IRequest<GetCustomerMembershipByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetCustomerMembershipByIdResponse
    {
        public CustomerMembershipModel CustomerMembership { get; set; }
    }

    public class GetCustomerMembershipByIdRequestHandler : IRequestHandler<GetCustomerMembershipByIdRequest, GetCustomerMembershipByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCustomerMembershipByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetCustomerMembershipByIdResponse> Handle(GetCustomerMembershipByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customerMembershipData = await _unitOfWork.CustomerMemberships.GetCustomerMembershipDetailByIdAsync(request.Id.Value, loggedUser.StoreId);

            ThrowError.Against(customerMembershipData == null, "Cannot find customer membership information");

            var customerMembership = _mapper.Map<CustomerMembershipModel>(customerMembershipData);

            return new GetCustomerMembershipByIdResponse
            {
                CustomerMembership = customerMembership
            };
        }
    }
}

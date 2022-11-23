using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Customer;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Account.Queries
{
    public class GetAccountAddressesByAccountIdRequest : IRequest<GetCustomerAddressesByAccountIdResponse>
    {
    }

    public class GetCustomerAddressesByAccountIdResponse
    {
        public IEnumerable<CustomerAddressModel> CustomerAddresses { get; set; }
    }

    public class GetCustomerAddressesByAccountIdRequestHandler : IRequestHandler<GetAccountAddressesByAccountIdRequest, GetCustomerAddressesByAccountIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetCustomerAddressesByAccountIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetCustomerAddressesByAccountIdResponse> Handle(GetAccountAddressesByAccountIdRequest request, CancellationToken cancellationToken)
        {
            var customer = _userProvider.GetLoggedCustomer();

            var customerAddresses = await _unitOfWork.AccountAddresses.GetAccountAddressesByAccountIdAsync(customer.Id ?? Guid.Empty)
                .AsNoTracking()
                .ProjectTo<CustomerAddressModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetCustomerAddressesByAccountIdResponse()
            {
                CustomerAddresses = customerAddresses
            };

            return response;
        }
    }
}

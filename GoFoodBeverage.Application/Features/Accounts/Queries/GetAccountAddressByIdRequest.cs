using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Customer;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Account.Queries
{
    public class GetAccountAddressByIdRequest : IRequest<GetCustomerAddressByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerAddressByIdResponse
    {
        public CustomerAddressModel CustomerAddress { get; set; }
    }

    public class GetCustomerAddressByIdRequestHandler : IRequestHandler<GetAccountAddressByIdRequest, GetCustomerAddressByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetCustomerAddressByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetCustomerAddressByIdResponse> Handle(GetAccountAddressByIdRequest request, CancellationToken cancellationToken)
        {
            var customer = _userProvider.GetLoggedCustomer();

            var customerAddress = await _unitOfWork.AccountAddresses.Find(cd => cd.Id == request.Id)
                .AsNoTracking()
                .ProjectTo<CustomerAddressModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var response = new GetCustomerAddressByIdResponse()
            {
                CustomerAddress = customerAddress
            };

            return response;
        }
    }
}

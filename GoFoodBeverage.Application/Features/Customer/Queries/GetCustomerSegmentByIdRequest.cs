using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Extensions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerSegmentByIdRequest : IRequest<GetCustomerSegmentByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetCustomerSegmentByIdResponse
    {
        public CustomerSegmentByIdModel CustomerSegment { get; set; }
    }

    public class GetCustomerSegmentByIdRequestHandler : IRequestHandler<GetCustomerSegmentByIdRequest, GetCustomerSegmentByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCustomerSegmentByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetCustomerSegmentByIdResponse> Handle(GetCustomerSegmentByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customerSegmentData = await _unitOfWork.CustomerSegments.GetCustomerSegmentDetailByIdAsync(request.Id.Value, loggedUser.StoreId);

            ThrowError.Against(customerSegmentData == null, "Cannot find customer segment information");

            var customerSegment = _mapper.Map<CustomerSegmentByIdModel>(customerSegmentData);

            return new GetCustomerSegmentByIdResponse
            {
                CustomerSegment = customerSegment
            };
        }
    }
}

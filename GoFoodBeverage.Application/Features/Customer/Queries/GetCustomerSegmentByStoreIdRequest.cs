using AutoMapper;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerSegmentInCurrentStoreRequest : IRequest<List<GetCustomerSegmentInCurrentStoreResponse>>
    {
        public Guid? StoreId { get; set; }
    }

    public class GetCustomerSegmentInCurrentStoreResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int TotalCustomer { get; set; }

        public int TotalEmail { get; set; }
    }

    public class GetCustomerSegmentByStoreIdRequestHandler : IRequestHandler<GetCustomerSegmentInCurrentStoreRequest, List<GetCustomerSegmentInCurrentStoreResponse>>
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public GetCustomerSegmentByStoreIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<List<GetCustomerSegmentInCurrentStoreResponse>> Handle(GetCustomerSegmentInCurrentStoreRequest request, CancellationToken cancellationToken)
        {
            // Get user information from the token.
            var loggedUser = await _userProvider.ProvideAsync();
            Guid? storeId = loggedUser.StoreId;
            if (request.StoreId.HasValue && request.StoreId.Value != Guid.Empty)
                storeId = request.StoreId.Value;
            var cutomerSegmentList = await _unitOfWork.CustomerSegments
                .GetAllCustomerSegmentsInStore(storeId.Value)
                .AsNoTracking()
                .ToListAsync();
            List<GetCustomerSegmentInCurrentStoreResponse> responses = _mapper.Map<List<GetCustomerSegmentInCurrentStoreResponse>>(cutomerSegmentList);

            return responses;
        }
    }
}

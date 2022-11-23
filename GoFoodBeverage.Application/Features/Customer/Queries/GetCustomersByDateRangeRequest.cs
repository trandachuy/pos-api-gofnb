using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomersByDateRangeRequest : IRequest<List<GetCustomersByDateRangeResponse>>
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid? BranchId { get; set; }
    }

    public class GetCustomersByDateRangeResponse
    {
        public Guid? Id { get; set; }

        public DateTime? CreatedTime { get; set; }
    }

    public class GetCustomersByDateRangeHandler : IRequestHandler<GetCustomersByDateRangeRequest, List<GetCustomersByDateRangeResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetCustomersByDateRangeHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<List<GetCustomersByDateRangeResponse>> Handle(GetCustomersByDateRangeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            DateTime startDate = request.StartDate.StartOfDay().ToUniversalTime();
            DateTime endDate = request.EndDate.EndOfDay().ToUniversalTime();

            var response = await _unitOfWork.Customers
                .Find(x => x.StoreId == loggedUser.StoreId
                    && x.CreatedTime.HasValue
                    && startDate <= x.CreatedTime.Value.Date && x.CreatedTime.Value.Date <= endDate
                    && (!request.BranchId.HasValue || x.BranchId == request.BranchId))
                .OrderBy(x => x.CreatedTime)
                .Select(x => new GetCustomersByDateRangeResponse
                {
                    Id = x.Id,
                    CreatedTime = x.CreatedTime
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return response;
        }
    }
}

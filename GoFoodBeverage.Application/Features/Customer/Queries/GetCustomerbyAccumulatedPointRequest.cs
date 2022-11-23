using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Customer;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using MoreLinq;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerbyAccumulatedPointRequest : IRequest<GetCustomerbyAccumulatedPointResponse>
    {
        public int AccumulatedPoint { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetCustomerbyAccumulatedPointResponse
    {
        public IEnumerable<CustomersModel> Customers { get; set; }

        public int Total { get; set; }
    }

    public class GetCustomerbyAccumulatedPointHandler : IRequestHandler<GetCustomerbyAccumulatedPointRequest, GetCustomerbyAccumulatedPointResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetCustomerbyAccumulatedPointHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetCustomerbyAccumulatedPointResponse> Handle(GetCustomerbyAccumulatedPointRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            var listCustomerPoints = await _unitOfWork.CustomerPoints.GetAllCustomerPointInStore(loggedUser.StoreId).ToListAsync();
            var lisCustomer = await _unitOfWork.Customers.GetCustomerByKeySearchInStore(null, loggedUser.StoreId).ToListAsync();

            customerMemberships = customerMemberships.OrderBy(x => x.AccumulatedPoint).ToList();
            var lenCustomerMemberships = customerMemberships.Count;
            var lisCustomerResponse = new List<Domain.Entities.Customer>();

            var customerMembershipAccumulatedPoint = customerMemberships.FirstOrDefault(x => x.AccumulatedPoint == request.AccumulatedPoint);
            var indexCustomerMembershipAccumulatedPoint = customerMemberships.IndexOf(customerMembershipAccumulatedPoint);
            if (indexCustomerMembershipAccumulatedPoint != (lenCustomerMemberships - 1))
            {
                var listCustomerIds = listCustomerPoints.Where(x => (x.AccumulatedPoint >= request.AccumulatedPoint) && (x.AccumulatedPoint < customerMemberships[indexCustomerMembershipAccumulatedPoint + 1]?.AccumulatedPoint)).Select(x => x.CustomerId).ToList();
                var listCustomersByAccumulatedPoint = lisCustomer.Where(x => listCustomerIds.Contains(x.Id)).ToList();
                lisCustomerResponse.AddRange(listCustomersByAccumulatedPoint);
            }
            else
            {
                var listCustomerIds = listCustomerPoints.Where(x => (x.AccumulatedPoint >= request.AccumulatedPoint)).Select(x => x.CustomerId).ToList();
                var listCustomersByAccumulatedPoint = lisCustomer.Where(x => listCustomerIds.Contains(x.Id)).ToList();
                lisCustomerResponse.AddRange(listCustomersByAccumulatedPoint);
            }

            var customersByPaging = lisCustomerResponse.OrderByDescending(p => p.CreatedTime).ToList().ToPagination(request.PageNumber, request.PageSize);
            var customerModels = _mapper.Map<List<CustomersModel>>(customersByPaging.Result);
            customerModels.ForEach(c =>
            {
                c.No = customerModels.IndexOf(c) + ((request.PageNumber - 1) * request.PageSize) + 1;
                c.AccumulatedPoint = listCustomerPoints.FirstOrDefault(x => x.CustomerId == c.Id)?.AccumulatedPoint;
            });

            var response = new GetCustomerbyAccumulatedPointResponse()
            {
                Customers = customerModels,
                Total = customersByPaging.Total
            };

            return response;
        }
    }
}

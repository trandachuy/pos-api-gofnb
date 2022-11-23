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
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomersRequest : IRequest<GetCustomersResponse>
    {
        public string keySearch { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetCustomersResponse
    {
        public IEnumerable<CustomersModel> Customers { get; set; }

        public int Total { get; set; }
    }

    public class GetCustomerByNameHandler : IRequestHandler<GetCustomersRequest, GetCustomersResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetCustomerByNameHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetCustomersResponse> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var customersByPaging = await _unitOfWork.Customers
                .GetCustomerByKeySearchInStore(request.keySearch, loggedUser.StoreId)
                .Include(x => x.CustomerPoint)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);

            var customerModels = _mapper.Map<List<CustomersModel>>(customersByPaging.Result);
            var customerPoint = customersByPaging.Result.Where(x => x.CustomerPoint != null).Select(x => x.CustomerPoint);

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            var checkExistClassicMember = customerMemberships.FirstOrDefault(x => x.AccumulatedPoint == 0 && x.Name == DefaultConstants.CLASSIC_MEMBER);
            if (checkExistClassicMember == null && customerMemberships.Count() == 0)
            {
                var customerMembershipModelAdd = new Domain.Entities.CustomerMembershipLevel { AccumulatedPoint = 0, Name = DefaultConstants.CLASSIC_MEMBER, StoreId = loggedUser.StoreId };
                await _unitOfWork.CustomerMemberships.AddAsync(customerMembershipModelAdd);
            }

            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            customerModels.ForEach(c =>
            {
                c.No = customerModels.IndexOf(c) + ((request.PageNumber - 1) * request.PageSize) + 1;
                c.Point = customerPoint.FirstOrDefault(x => x.CustomerId == c.Id)?.AccumulatedPoint;
                foreach (var membership in customerMemberships)
                {
                    if (c.Point >= membership.AccumulatedPoint)
                    {
                        c.Rank = membership.Name;
                        c.Color = membership.Color;
                        break;
                    }
                }
            });
            var response = new GetCustomersResponse()
            {
                Customers = customerModels,
                Total = customersByPaging.Total
            };

            return response;
        }
    }
}

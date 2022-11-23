using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Customer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetListCustomersBySegmentRequest : IRequest<GetListCustomersBySegmentResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? CustomerSegmentId { get; set; }
    }

    public class GetListCustomersBySegmentResponse
    {
        public IEnumerable<CustomerDataBySegmentModel> Customers { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetListCustomersBySegmentRequestHandler : IRequestHandler<GetListCustomersBySegmentRequest, GetListCustomersBySegmentResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListCustomersBySegmentRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetListCustomersBySegmentResponse> Handle(GetListCustomersBySegmentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customers = _unitOfWork.Customers.GetAllCustomersInStore(loggedUser.StoreId.Value).Include(o => o.CustomerPoint).AsNoTracking();

            var customerPoint = customers.Where(x => x.CustomerPoint != null).Select(x => x.CustomerPoint);

            var memberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId)
                .Select(x => new
                {
                    x.Name,
                    x.AccumulatedPoint
                })
                .ToListAsync(cancellationToken);

            if (customers != null)
            {
                if (request.CustomerSegmentId != null)
                {
                    /// Find customers in segments
                    var customerIdsInCustomerSegment = _unitOfWork.CustomerCustomerSegments
                        .Find(m => m.StoreId == loggedUser.StoreId.Value && m.CustomerSegmentId == request.CustomerSegmentId)
                        .Select(m => m.CustomerId);

                    customers = customers.Where(x => customerIdsInCustomerSegment.Contains(x.Id));
                }


                if (!string.IsNullOrEmpty(request.KeySearch))
                {
                    string keySearch = request.KeySearch.Trim().ToLower();
                    customers = customers.Where(c => c.FullName.Trim().ToLower().Contains(keySearch) || c.PhoneNumber.Trim().ToLower().Contains(keySearch));
                }
            }

            var allCustomersInStore = await customers.OrderByDescending(p => p.CreatedTime).ToPaginationAsync(request.PageNumber, request.PageSize);
            var pagingResult = allCustomersInStore.Result;
            var customerListResponse = _mapper.Map<List<CustomerDataBySegmentModel>>(pagingResult);
            customerListResponse.ForEach(c =>
            {
                c.No = customerListResponse.IndexOf(c) + ((request.PageNumber - 1) * request.PageSize) + 1;
                c.Point = customerPoint.FirstOrDefault(x => x.CustomerId == c.Id)?.AccumulatedPoint;
                foreach (var membership in memberships)
                {
                    if (c.Point >= membership.AccumulatedPoint)
                    {
                        c.Rank = membership.Name;
                        break;
                    }
                }
            });

            var response = new GetListCustomersBySegmentResponse()
            {
                PageNumber = request.PageNumber,
                Total = allCustomersInStore.Total,
                Customers = customerListResponse
            };

            return response;
        }
    }
}

using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Product;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Models.Customer;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetCustomerSegmentsRequest : IRequest<GetCustomerSegmentsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetCustomerSegmentsResponse
    {
        public IEnumerable<CustomerSegmentModel> CustomerSegments { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetCustomerSegmentsRequestHandler : IRequestHandler<GetCustomerSegmentsRequest, GetCustomerSegmentsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomerSegmentActivityService _customerSegmentActivityService;

        public GetCustomerSegmentsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICustomerSegmentActivityService customerSegmentActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerSegmentActivityService = customerSegmentActivityService;
        }

        public async Task<GetCustomerSegmentsResponse> Handle(GetCustomerSegmentsRequest request, CancellationToken cancellationToken)
        {
            await _customerSegmentActivityService.ClassificationCustomersByCustomerSegmentAsync();

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var allCustomerSegmentsInStore = new PagingExtensions.Pager<CustomerSegment>(new List<CustomerSegment>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                allCustomerSegmentsInStore = await _unitOfWork.CustomerSegments
                    .GetAllCustomerSegmentsInStore(loggedUser.StoreId.Value)
                    .Include(s => s.CustomerCustomerSegments)
                    .ThenInclude(cs => cs.Customer)
                    .OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                allCustomerSegmentsInStore = await _unitOfWork.CustomerSegments
                   .GetAllCustomerSegmentsInStore(loggedUser.StoreId.Value)
                   .Where(s => s.Name.ToLower().Contains(keySearch))
                   .Include(s => s.CustomerCustomerSegments)
                   .ThenInclude(cs => cs.Customer)
                   .OrderByDescending(p => p.CreatedTime)
                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listAllCustomerSegmentInStore = allCustomerSegmentsInStore.Result;
            var customerSegmentListResponse = _mapper.Map<List<CustomerSegmentModel>>(listAllCustomerSegmentInStore);
            customerSegmentListResponse.ForEach(s =>
            {
                var customerSegment = listAllCustomerSegmentInStore.FirstOrDefault(i => i.Id == s.Id);
                var customers = customerSegment.CustomerCustomerSegments.Select(cs => cs.Customer);
                s.Customers = _mapper.Map<IEnumerable<CustomerDataBySegmentModel>>(customers);
                s.Member = s.Customers.Count();
                s.No = customerSegmentListResponse.IndexOf(s) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetCustomerSegmentsResponse()
            {
                PageNumber = request.PageNumber,
                Total = allCustomerSegmentsInStore.Total,
                CustomerSegments = customerSegmentListResponse
            };

            return response;
        }
    }
}

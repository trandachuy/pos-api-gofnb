using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Order;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderByIdRequest : IRequest<GetOrderByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetOrderByIdResponse
    {
        public OrderDetailDataById Order { get; set; }
    }

    public class GetOrderByIdRequestHandler : IRequestHandler<GetOrderByIdRequest, GetOrderByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetOrderByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetOrderByIdResponse> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var order = await _unitOfWork.Orders.GetOrderDetailDataById(request.Id, loggedUser.StoreId)
                              .AsNoTracking()
                              .ProjectTo<OrderDetailDataById>(_mapperConfiguration)
                              .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            order.BranchName = _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(order.BranchId).FirstOrDefault().Name;

            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(loggedUser.StoreId).ToListAsync();
            customerMemberships = customerMemberships.OrderByDescending(x => x.AccumulatedPoint).ToList();
            foreach (var membership in customerMemberships)
            {
                if (order?.Customer?.AccumulatedPoint >= membership?.AccumulatedPoint)
                {
                    order.Customer.Rank = membership.Name;
                    break;
                }
            }

            order.Reason = await _unitOfWork.OrderHistories
                .Find(oh => oh.OrderId == order.Id && oh.StoreId == loggedUser.StoreId && oh.CancelReason != null)
                .OrderByDescending(oh => oh.CreatedTime)
                .AsNoTracking()
                .Select(oh => oh.CancelReason)
                .FirstOrDefaultAsync();

            var response = new GetOrderByIdResponse()
            {
                Order = order
            };

            return response;
        }
    }
}

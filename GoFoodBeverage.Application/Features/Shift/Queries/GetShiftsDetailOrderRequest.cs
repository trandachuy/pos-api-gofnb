using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using System;
using GoFoodBeverage.Models.Order;

namespace GoFoodBeverage.Application.Features.Shift.Queries
{
    public class GetShiftsDetailOrderRequest : IRequest<GetShiftDetailOrderResponse>
    {
        public Guid? ShiftId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetShiftDetailOrderResponse
    {
        public IEnumerable<OrderModel> Orders { get; set; }

        public int Total { get; set; }
    }

    public class GetShiftDetailOrderHandler : IRequestHandler<GetShiftsDetailOrderRequest, GetShiftDetailOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _iDateTimeService;

        public GetShiftDetailOrderHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IDateTimeService iDateTimeService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _iDateTimeService = iDateTimeService;
        }

        public async Task<GetShiftDetailOrderResponse> Handle(GetShiftsDetailOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var listOrderDatas = _unitOfWork.Orders
                .GetAllOrdersInStore(loggedUser.StoreId)
                .Include(o => o.Customer).ThenInclude(c => c.CustomerPoint)
                .Where(x=>x.ShiftId == request.ShiftId)
                .AsNoTracking();

            var listOrderOrdered = listOrderDatas.OrderByDescending(o => o.CreatedTime);
            var listOrderByPaging = await listOrderOrdered.ToPaginationAsync(request.PageNumber, request.PageSize);
            var listOrderModels = _mapper.Map<IEnumerable<OrderModel>>(listOrderByPaging.Result);         

            var response = new GetShiftDetailOrderResponse()
            {
                Orders = listOrderModels,
                Total = listOrderByPaging.Total
            };

            return response;
        }
    }
}

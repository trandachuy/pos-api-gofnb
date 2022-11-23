using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Order;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetStatisticalDataRequest : IRequest<GetStatisticalDataResponse>
    {
        public EnumSegmentTime SegmentTimeOption { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid? BranchId { get; set; }

        public bool IsAverage { get; set; }
    }

    public class GetStatisticalDataResponse
    {
        public List<OrderStatisticModel> OrderData { get; set; }
    }

    public class GetStatisticalDataHandler : IRequestHandler<GetStatisticalDataRequest, GetStatisticalDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetStatisticalDataHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetStatisticalDataResponse> Handle(GetStatisticalDataRequest request, CancellationToken cancellationToken)
        {
            DateTime startDate = request.StartDate;
            DateTime endDate = request.EndDate;

            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var data = _unitOfWork.Orders.CountOrderByStoreBranchId(loggedUser.StoreId, request.BranchId);

            var orderDataByDatetime = data.Where(order => order.CreatedTime >= startDate && order.CreatedTime <= endDate)
                .Select(m => new OrderStatisticModel()
                {
                    Id = m.Id,
                    CreatedTime = m.CreatedTime.Value,
                    PriceAfterDiscount = m.PriceAfterDiscount
                }).ToList();

            var response = new GetStatisticalDataResponse()
            {
                OrderData = orderDataByDatetime
            };

            return response;
        }
    }
}

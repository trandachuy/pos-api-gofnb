using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Fee;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.POS.Application.Features.Fees.Queries
{
    public class GetFeesRequest : IRequest<GetFeesResponse>
    {
        public int OrderType { get; set; }
    }

    public class GetFeesResponse
    {
        public IEnumerable<FeeModel> AllFeesActive { get; set; }

        public IEnumerable<FeeModel> FeesDeActive { get; set; }

        public IEnumerable<FeeModel> FeesAutoApplied { get; set; }
    }

    public class GetFeesHandler : IRequestHandler<GetFeesRequest, GetFeesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOrderService _orderService;

        public GetFeesHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration,
            IDateTimeService iDateTimeService,
            IOrderService orderService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
            _dateTimeService = iDateTimeService;
            _orderService = orderService;
        }

        public async Task<GetFeesResponse> Handle(GetFeesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var currentDate = _dateTimeService.NowUtc.Date;
            var listFeeIdByServingTypes = _unitOfWork.FeeServingTypes.Find(x => x.StoreId == loggedUser.StoreId && (int)x.OrderServingType == request.OrderType).Select(x => x.FeeId);
            var listFeeIds = _unitOfWork.FeeBranches.Find(x => x.StoreId == loggedUser.StoreId && x.BranchId == loggedUser.BranchId && listFeeIdByServingTypes.Contains(x.FeeId)).Select(x => x.FeeId);

            var allFeesActive = await _unitOfWork.Fees
                .GetAllFeesByListIdInStore(listFeeIds, loggedUser.StoreId.Value)
                .ProjectTo<FeeModel>(_mapperConfiguration)
                .AsNoTracking()
                .Where(f => ((f.StartDate == null && f.EndDate == null) ||
                    ((f.StartDate.HasValue && f.EndDate.HasValue) && (currentDate >= f.StartDate.Value.Date && currentDate <= f.EndDate.Value.Date)) ||
                    (f.StartDate == null && currentDate <= f.EndDate.Value.Date) ||
                    (currentDate >= f.StartDate.Value.Date && f.EndDate == null)) && (f.IsStopped != true)
                )
                .ToListAsync(cancellationToken: cancellationToken);

            var feesAutoApplied = allFeesActive.Where(x => x.IsAutoApplied == true);
            var reponse = new GetFeesResponse
            {
                AllFeesActive = allFeesActive,
                FeesDeActive = allFeesActive.Except(feesAutoApplied),
                FeesAutoApplied = feesAutoApplied
            };

            return reponse;
        }
    }
}

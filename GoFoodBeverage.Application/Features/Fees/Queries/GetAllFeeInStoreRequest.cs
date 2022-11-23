using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Fee;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using MoreLinq;
using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Application.Features.Fees.Queries
{
    public class GetAllFeeInStoreRequest : IRequest<GetAllFeeInStoreResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetAllFeeInStoreResponse
    {
        public IEnumerable<FeeModel> Fees { get; set; }

        public int Total { get; set; }
    }

    public class GetAllFeeInStoreRequestHandler : IRequestHandler<GetAllFeeInStoreRequest, GetAllFeeInStoreResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTime;

        public GetAllFeeInStoreRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserProvider userProvider,
            IDateTimeService dateTime)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
            _dateTime = dateTime;
        }

        public async Task<GetAllFeeInStoreResponse> Handle(GetAllFeeInStoreRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var fees = await _unitOfWork.Fees
                .GetAllFeesInStore(loggedUser.StoreId)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync(cancellationToken: cancellationToken);

            var feesByPaging = fees.ToPagination(request.PageNumber, request.PageSize);
            var feeModels = _mapper.Map<IEnumerable<FeeModel>>(feesByPaging.Result);

            // Manually mapping
            feeModels.ForEach(item =>
            {
                if (item.IsStopped)
                {
                    item.StatusId = (int)EnumFeeStatus.Finished;
                }
                else
                {
                    item.StatusId = (int)GetFeeStatus(item.StartDate, item.EndDate);
                }
            });

            var response = new GetAllFeeInStoreResponse()
            {
                Fees = feeModels,
                Total = fees.Count
            };
            return response;
        }

        private EnumFeeStatus GetFeeStatus(DateTime? startDate, DateTime? dueDate)
        {
            if (startDate.HasValue && dueDate.HasValue)
            {
                var result = _dateTime.NowUtc.CompareTo(startDate.Value);

                if (result < 0)
                {
                    return EnumFeeStatus.Scheduled;
                }
                else if (result == 0)
                {
                    return EnumFeeStatus.Active;
                }
                else
                {
                    if (dueDate.HasValue)
                    {
                        return _dateTime.NowUtc.CompareTo(dueDate.Value) >= 0 ? EnumFeeStatus.Finished : EnumFeeStatus.Active;
                    }
                    else
                    {
                        return EnumFeeStatus.Active;
                    }
                }
            }
            else
            {
                var currentDate = _dateTime.NowUtc;
                // If start date and due date are null => Always active
                if (!startDate.HasValue && !dueDate.HasValue)
                {
                    return EnumFeeStatus.Active;
                }
                if (!startDate.HasValue)
                {
                    return currentDate <= dueDate.Value ? EnumFeeStatus.Active : EnumFeeStatus.Finished;
                }
                else
                {
                    if(currentDate == startDate.Value)
                        return EnumFeeStatus.Active;

                    if (currentDate < startDate.Value)
                        return EnumFeeStatus.Scheduled;

                    return EnumFeeStatus.Active;
                }
            }
        }
    }
}

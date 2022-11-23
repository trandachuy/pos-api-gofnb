using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Promotion;
using System;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Promotions.Queries
{
    public class GetPromotionsRequest : IRequest<GetPromotionsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetPromotionsResponse
    {
        public IEnumerable<GetPromotionsInStoreModel> Promotions { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetPromotionsRequestHandler : IRequestHandler<GetPromotionsRequest, GetPromotionsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTime;

        public GetPromotionsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IDateTimeService dateTime)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTime = dateTime;
        }

        public async Task<GetPromotionsResponse> Handle(GetPromotionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var allPromotionInStore = new PagingExtensions.Pager<Promotion>(new List<Promotion>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                allPromotionInStore = await _unitOfWork.Promotions
                                   .GetAllPromotionInStore(loggedUser.StoreId.Value)
                                   .AsNoTracking()
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                allPromotionInStore = await _unitOfWork.Promotions
                                   .GetAllPromotionInStore(loggedUser.StoreId.Value)
                                   .Where(s => s.Name.ToLower().Contains(keySearch))
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listAllPromotionInStore = allPromotionInStore.Result;
            var promotionListResponse = _mapper.Map<List<GetPromotionsInStoreModel>>(listAllPromotionInStore);

            // Manually mapping
            promotionListResponse.ForEach(item =>
            {
                if (item.IsStopped)
                {
                    item.StatusId = (int)EnumPromotionStatus.Finished;
                }
                else
                {
                    item.StatusId = (int)GetPromotionStatus(item.StartDate, item.EndDate);
                }
            });

            var response = new GetPromotionsResponse()
            {
                PageNumber = request.PageNumber,
                Total = allPromotionInStore.Total,
                Promotions = promotionListResponse
            };

            return response;
        }

        private EnumPromotionStatus GetPromotionStatus(DateTime startDate, DateTime? dueDate)
        {
            var result = _dateTime.NowUtc.Date.CompareTo(startDate.Date);

            if (result < 0)
            {
                return EnumPromotionStatus.Scheduled;
            }
            else if (result == 0)
            {
                return EnumPromotionStatus.Active;
            }
            else
            {
                if (dueDate.HasValue)
                {
                    return _dateTime.NowUtc.Date.CompareTo(dueDate.Value.Date) >= 0 ? EnumPromotionStatus.Finished : EnumPromotionStatus.Active;
                }
                else
                {
                    return EnumPromotionStatus.Active;
                }
            }
        }
    }
}

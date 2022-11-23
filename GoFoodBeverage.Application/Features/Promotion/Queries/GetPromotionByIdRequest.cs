using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Promotion;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Promotions.Queries
{
    public class GetPromotionByIdRequest : IRequest<GetPromotionByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetPromotionByIdResponse
    {
        public GetPromotionByIdModel Promotion { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class GetPromotionByIdRequestHandler : IRequestHandler<GetPromotionByIdRequest, GetPromotionByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTime;

        public GetPromotionByIdRequestHandler(
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

        public async Task<GetPromotionByIdResponse> Handle(GetPromotionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var promotion = await _unitOfWork.Promotions
                .GetPromotionByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);
            
            var isSuccess = true;
            if (promotion == null)
            {
                isSuccess = false;
            }

            var promotionDetail = _mapper.Map<GetPromotionByIdModel>(promotion);

            if (promotion.IsStopped.HasValue && promotion.IsStopped.Value)
            {
                promotionDetail.StatusId = (int)EnumFeeStatus.Finished;
            }
            else
            {
                promotionDetail.StatusId = (int)GetPromotionStatus(promotion.StartDate, promotion.EndDate);
            }

            return new GetPromotionByIdResponse()
            {
                Promotion = promotionDetail,
                IsSuccess = isSuccess
            };

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

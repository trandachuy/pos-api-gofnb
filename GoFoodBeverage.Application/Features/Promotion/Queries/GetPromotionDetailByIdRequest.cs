using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Promotion;

namespace GoFoodBeverage.Application.Features.Promotions.Queries
{
    public class GetPromotionDetailByIdRequest : IRequest<GetPromotionDetailByIdResponse>
    {
        public Guid StoreId { get; set; }

        public Guid PromotionId { get; set; }
    }

    public class GetPromotionDetailByIdResponse
    {
        public PromotionDetailModel Promotion { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class GetPromotionDetailByIdRequestHandler : IRequestHandler<GetPromotionDetailByIdRequest, GetPromotionDetailByIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetPromotionDetailByIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork
        )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetPromotionDetailByIdResponse> Handle(GetPromotionDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var promotion = await _unitOfWork.Promotions.GetPromotionByIdInStoreAsync(request.PromotionId, request.StoreId);
            
            var isSuccess = true;
            if (promotion == null)
            {
                isSuccess = false;
            }

            var promotionDetail = _mapper.Map<PromotionDetailModel>(promotion);

            return new GetPromotionDetailByIdResponse()
            {
                IsSuccess = isSuccess,
                Promotion = promotionDetail
            };

        }
    }

}

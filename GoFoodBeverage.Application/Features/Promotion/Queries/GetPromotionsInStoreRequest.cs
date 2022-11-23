using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Promotion;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Promotions.Queries
{
    public class GetPromotionsInStoreRequest : IRequest<GetPromotionsInStoreResponse>
    {
        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }

        public DateTime? CurrentDate { get; set; }
    }

    public class GetPromotionsInStoreResponse
    {
        public List<GetPromotionsInBranchModel> Promotions { get; set; }
    }

    public class GetPromotionsInStoreRequestHandler : IRequestHandler<GetPromotionsInStoreRequest, GetPromotionsInStoreResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPromotionsInStoreRequestHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetPromotionsInStoreResponse> Handle(GetPromotionsInStoreRequest request, CancellationToken cancellationToken)
        {
            var allPromotionsInStore = await _unitOfWork.Promotions
                                   .GetAllPromotionInStore(request.StoreId)
                                   .Where(p => p.IsStopped == false &&  request.CurrentDate.HasValue && ((p.EndDate == null && p.StartDate != null) || (p.EndDate != null && p.EndDate.Value.Date >= request.CurrentDate.Value.Date)))
                                   .Include(p => p.PromotionBranches)
                                   .AsNoTracking()
                                   .OrderByDescending(p => p.CreatedTime)
                                   .Select(p => new { p.Id, p.StoreId, p.Name, p.PromotionTypeId, p.IsPercentDiscount, p.PercentNumber, p.MaximumDiscountAmount, p.StartDate, p.EndDate, p.IsSpecificBranch ,p.PromotionBranches})
                                   .ToListAsync();

            var allPromotionsInBranch = new List<GetPromotionsInBranchModel>();

            allPromotionsInStore.ForEach(promotion =>
            {
                var promotionBranch = promotion.PromotionBranches.FirstOrDefault(pb => pb.BranchId == request.BranchId);
                if (!promotion.IsSpecificBranch || promotionBranch != null)
                {
                    var promotionReponse = new GetPromotionsInBranchModel()
                    {
                        Id = promotion.Id,
                        StoreId = promotion.StoreId,
                        Name = promotion.Name,
                        PromotionTypeId = promotion.PromotionTypeId,
                        IsPercentDiscount = promotion.IsPercentDiscount,
                        PercentNumber = promotion.PercentNumber,
                        MaximumDiscountAmount = promotion.MaximumDiscountAmount,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate
                    };
                    allPromotionsInBranch.Add(promotionReponse);
                }
            });

            var response = new GetPromotionsInStoreResponse()
            {
                Promotions = allPromotionsInBranch.OrderBy(x => x.EndDate ?? DateTime.MaxValue).ToList()
            };

            return response;
        }
    }
}

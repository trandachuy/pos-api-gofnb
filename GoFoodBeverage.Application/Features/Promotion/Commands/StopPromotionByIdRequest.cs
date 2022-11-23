using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace GoFoodBeverage.Application.Features.Promotions.Commands
{
    public class StopPromotionByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class StopPromotionByIdRequestHandler : IRequestHandler<StopPromotionByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public StopPromotionByIdRequestHandler(
            IUserProvider userProvider,
           IUnitOfWork unitOfWork,
           MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<bool> Handle(StopPromotionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var promotion = await _unitOfWork.Promotions.Find(p => p.StoreId == loggedUser.StoreId && p.Id == request.Id).FirstOrDefaultAsync();
            ThrowError.Against(promotion == null, "Cannot find promotion information");

            var modifiedPromotion = UpdatePromotion(promotion, loggedUser.AccountId.Value);
            await _unitOfWork.Promotions.UpdateAsync(modifiedPromotion);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public static Promotion UpdatePromotion(Promotion promotion, Guid accountId)
        {
            promotion.IsStopped = true;
            promotion.LastSavedUser = accountId;

            return promotion;
        }
    }
}

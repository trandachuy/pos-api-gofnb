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

namespace GoFoodBeverage.Application.Features.Fees.Commands
{
    public class StopFeeByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class StopFeeByIdRequestHandler : IRequestHandler<StopFeeByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public StopFeeByIdRequestHandler(
            IUserProvider userProvider,
           IUnitOfWork unitOfWork,
           MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<bool> Handle(StopFeeByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var fee = await _unitOfWork.Fees.Find(p => p.StoreId == loggedUser.StoreId && p.Id == request.Id).FirstOrDefaultAsync();
            ThrowError.Against(fee == null, "Cannot find fee information");

            var modifiedFee = UpdateFee(fee, loggedUser.AccountId.Value);
            await _unitOfWork.Fees.UpdateAsync(modifiedFee);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public static Fee UpdateFee(Fee fee, Guid accountId)
        {
            fee.IsStopped = true;
            fee.LastSavedUser = accountId;

            return fee;
        }
    }
}

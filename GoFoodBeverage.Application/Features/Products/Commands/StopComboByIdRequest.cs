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
    public class StopComboByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class StopComboByIdRequestHandler : IRequestHandler<StopComboByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public StopComboByIdRequestHandler(
            IUserProvider userProvider,
           IUnitOfWork unitOfWork,
           MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<bool> Handle(StopComboByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var combo = await _unitOfWork.Combos.Find(p => p.StoreId == loggedUser.StoreId && p.Id == request.Id).FirstOrDefaultAsync();
            ThrowError.Against(combo == null, "Cannot find combo information");

            var modifiedCombo = UpdateCombo(combo, loggedUser.AccountId.Value);
            await _unitOfWork.Combos.UpdateAsync(modifiedCombo);

            return true;
        }

        public static Combo UpdateCombo(Combo combo, Guid accountId)
        {
            combo.IsStopped = true;
            combo.LastSavedUser = accountId;

            return combo;
        }
    }
}

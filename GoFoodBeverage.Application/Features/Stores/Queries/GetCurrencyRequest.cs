using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetCurrencyRequest : IRequest<string>
    {
        
    }

    public class GetCurrencyRequestHandler : IRequestHandler<GetCurrencyRequest, string>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrencyRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(GetCurrencyRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId);

            return store.Currency.Code;
        }
    }
}

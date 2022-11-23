using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class ActivateAccountStoreRequest : IRequest<bool>
    {
    }

    public class ActivateAccountStoreRequestHandler : IRequestHandler<ActivateAccountStoreRequest, bool>
    {
        private IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public ActivateAccountStoreRequestHandler(IUnitOfWork unitOfWork, IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(ActivateAccountStoreRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var store = await _unitOfWork.Stores.Find(a => a.Id == loggedUser.StoreId).FirstOrDefaultAsync();
            store.IsActivated = true;
            await _unitOfWork.Stores.UpdateAsync(store);

            return true;
        }
    }
}

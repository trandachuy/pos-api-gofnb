using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.FavoriteStores.Commands
{
    public class AddStoreFavoriteStoresRequest : IRequest<bool>
    {
        public Guid StoreId { get; set; }

        public Guid CustomerId { get; set; }
    }

    public class AddStoreLeaveFavoriteStoresRequestHandler : IRequestHandler<AddStoreFavoriteStoresRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddStoreLeaveFavoriteStoresRequestHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddStoreFavoriteStoresRequest request, CancellationToken cancellationToken)
        {

            var store = await _unitOfWork.Stores.GetStoreByIdAsync(request.StoreId);
            ThrowError.Against(store == null, "Cannot find store information");

            var customer = await _unitOfWork.Accounts.Find(c => c.Id == request.CustomerId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(customer == null, "Cannot find customer information");

            var addFavoriteStore = AddFavoriteStore(request);
            await _unitOfWork.FavoriteStores.AddAsync(addFavoriteStore);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static FavoriteStore AddFavoriteStore(AddStoreFavoriteStoresRequest request)
        {
            var newFavoriteStore = new FavoriteStore()
            {
                StoreId = request.StoreId,
                AccountId = request.CustomerId,
            };

            return newFavoriteStore;
        }
    }
}

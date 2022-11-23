using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.FavoriteStores.Commands
{
    public class RemoveStoreLeaveFavoriteStoresRequest : IRequest<bool>
    {
        public Guid StoreId { get; set; }

        public Guid CustomerId { get; set; }
    }

    public class RemoveStoreLeaveFavoriteStoresRequestHandler : IRequestHandler<RemoveStoreLeaveFavoriteStoresRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveStoreLeaveFavoriteStoresRequestHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RemoveStoreLeaveFavoriteStoresRequest request, CancellationToken cancellationToken)
        {

            var favoriteStore = await _unitOfWork.FavoriteStores.Find(o => o.AccountId == request.CustomerId && o.StoreId == request.StoreId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (favoriteStore == null)
            {
                return false;
            }

            await _unitOfWork.FavoriteStores.RemoveAsync(favoriteStore);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Promotions.Commands
{
    public class DeletePromotionByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeletePromotionRequestHandler : IRequestHandler<DeletePromotionByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromotionRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePromotionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialCategory = await _unitOfWork.Promotions.GetPromotionByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(materialCategory == null, "Promotion is not found");

            await _unitOfWork.Promotions.RemoveAsync(materialCategory);

            return true;
        }
    }

}

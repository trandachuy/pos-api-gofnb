using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class DeleteProductPriceMaterialByMaterialIdRequest : IRequest<bool>
    {
        public Guid MaterialId { get; set; }
    }

    public class DeleteProductPriceMaterialByMaterialIdHandler : IRequestHandler<DeleteProductPriceMaterialByMaterialIdRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteProductPriceMaterialByMaterialIdHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteProductPriceMaterialByMaterialIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productPriceMaterials = await  _unitOfWork.ProductPriceMaterials
                .Find(m => m.StoreId == loggedUser.StoreId && m.MaterialId == request.MaterialId)
                .ToListAsync(cancellationToken:cancellationToken);
            
            try
            {
                await _unitOfWork.ProductPriceMaterials.RemoveRangeAsync(productPriceMaterials);
            }
            catch 
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.PurchaseOrders.Queries;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class UpdateMaterialStatusRequest : IRequest<UpdateMaterialStatusResponse>
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateMaterialStatusResponse
    {
        public bool IsSuccess { get; set; }

        public GetPurchaseOrdersByMaterialIdResponse Data { get; set; }
    }

    public class UpdateMaterialStatusRequestHandler : IRequestHandler<UpdateMaterialStatusRequest, UpdateMaterialStatusResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public UpdateMaterialStatusRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMediator mediator
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<UpdateMaterialStatusResponse> Handle(UpdateMaterialStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var material = await _unitOfWork.Materials.Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.Id).FirstOrDefaultAsync();
            ThrowError.Against(material == null, "Material is not found");

            var response = new UpdateMaterialStatusResponse()
            {
                IsSuccess = true
            };

            /// Activate material
            material.IsActive = true;

            /// If the material is NOT busy (used for product || used for purchase order) => deactivate material
            if(request.IsActive == false)
            {
                var getPurchaseOrdersByMaterialIdResponse = await _mediator.Send(new GetPurchaseOrdersByMaterialIdRequest() { MaterialId = request.Id }, cancellationToken);
                var isBusy = getPurchaseOrdersByMaterialIdResponse.IsOpenProduct || getPurchaseOrdersByMaterialIdResponse.IsOpenProduct;
                if(isBusy)
                {
                    response.IsSuccess = false;
                    response.Data = getPurchaseOrdersByMaterialIdResponse;
                }
                else
                {
                    material.IsActive = false;
                }
            }

            _unitOfWork.Materials.Update(material);
            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}

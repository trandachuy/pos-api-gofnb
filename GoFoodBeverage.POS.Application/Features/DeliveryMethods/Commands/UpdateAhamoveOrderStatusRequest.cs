using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.POS.Models.DeliveryMethod;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.DeliveryMethods.Commands
{
    public class UpdateAhamoveOrderStatusRequest : UpdateAhamoveStatusRequestModel, IRequest<UpdateAhamoveOrderStatusResponse>
    {
    }

    public class UpdateAhamoveOrderStatusResponse
    {
    }

    public class UpdateAhamoveOrderStatusRequestHandler : IRequestHandler<UpdateAhamoveOrderStatusRequest, UpdateAhamoveOrderStatusResponse>
    {
        private readonly IDeliveryService _deliveryService;

        public UpdateAhamoveOrderStatusRequestHandler(
            IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<UpdateAhamoveOrderStatusResponse> Handle(UpdateAhamoveOrderStatusRequest request, CancellationToken cancellationToken)
        {
            await _deliveryService.UpdateOrderAhamoveStatusAsync(request, cancellationToken);

            return new UpdateAhamoveOrderStatusResponse();
        }
    }
}

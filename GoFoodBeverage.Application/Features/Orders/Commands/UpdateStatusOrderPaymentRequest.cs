using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Application.Features.Orders.Commands
{
    public class UpdateStatusOrderPaymentRequest : IRequest<UpdateOrderResponse>
    {
        public Guid OrderId { get; set; }

        public EnumOrderStatus OrderStatus { get; set; }

        public EnumOrderPaymentStatus OrderPaymentStatusId { get; set; }
    }

    public class UpdateOrderResponse
    {
        public bool Success { get; set; }

        public Guid OrderId { get; set; }

        public Guid BranchId { get; set; }
    }

    public class UpdateStatusOrderPaymentRequestHandler : IRequestHandler<UpdateStatusOrderPaymentRequest, UpdateOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStatusOrderPaymentRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateOrderResponse> Handle(UpdateStatusOrderPaymentRequest request, CancellationToken cancellationToken)
        {
            var dataToResponse = new UpdateOrderResponse()
            {
                Success = true
            };

            var order = await _unitOfWork.Orders.Find(o => o.Id == request.OrderId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if(order == null)
            {
                dataToResponse.Success = false;
            } else
            {
                order.StatusId = request.OrderStatus;
                order.OrderPaymentStatusId = request.OrderPaymentStatusId;

                await _unitOfWork.Orders.UpdateAsync(order);

                dataToResponse.OrderId = order.Id;
                dataToResponse.BranchId = order.BranchId.Value;
            }

            return dataToResponse;
        }
    }
}

using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusRequest : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public EnumOrderStatus Status { get; set; }

    }

    public class UpdateOrderStatusRequestHandle : IRequestHandler<UpdateOrderStatusRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderStatusRequestHandle(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = await _unitOfWork.Orders.GetAll().FirstOrDefaultAsync(order => order.Id == request.OrderId);

                    if (order != null)
                    {
                        order.StatusId = request.Status;
                        await _unitOfWork.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

        }
    }
}

using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class DeleteCustomerByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerRequestHandler : IRequestHandler<DeleteCustomerByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteCustomerRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteCustomerByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customer = await  _unitOfWork.Customers
                .GetCustomerByKeySearchInStore(null,loggedUser.StoreId)
                .Include(p=>p.CustomerPoint)
                .Include(x=>x.Address)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
            if (customer == null) return false;

            try
            {
                await _unitOfWork.Customers.RemoveAsync(customer);
                await _mediator.Send(new CreateStaffActivitiesRequest()
                {
                    ActionGroup = EnumActionGroup.Customer,
                    ActionType = EnumActionType.Deleted,
                    ObjectId = customer.Id,
                    ObjectName = string.IsNullOrEmpty(customer.FullName) ? $"{customer.LastName} {customer.FirstName}" : customer.FullName,
                    ObjectThumbnail = customer.Thumbnail
                });

                return true;
            }
            catch (Exception)
            {
                /// If the customer has orders => cannot delete this customer
                throw new Exception($"Cannot delete customer {customer.FullName}");
            }
        }
    }
}

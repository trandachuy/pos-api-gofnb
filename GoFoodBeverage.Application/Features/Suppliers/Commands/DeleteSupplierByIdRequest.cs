using GoFoodBeverage.Application.Features.Staffs.Commands;
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
namespace GoFoodBeverage.Application.Features.Suppliers.Commands
{
    public class DeleteSupplierByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteSupplierRequestHandler : IRequestHandler<DeleteSupplierByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public DeleteSupplierRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(DeleteSupplierByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var supplier = await _unitOfWork.Suppliers.Find(s => s.StoreId == loggedUser.StoreId && s.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (supplier == null)
            {
                return false;
            }

            await _unitOfWork.Suppliers.RemoveAsync(supplier);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Supplier,
                ActionType = EnumActionType.Deleted,
                ObjectId = supplier.Id,
                ObjectName = supplier.Name
            });

            return true;
        }
    }
}

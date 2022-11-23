using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.PurchaseOrders.Commands
{
    public class ApprovePurchaseOrderStatusByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ApprovePurchaseOrderStatusByIdRequestHandler : IRequestHandler<ApprovePurchaseOrderStatusByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public ApprovePurchaseOrderStatusByIdRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(ApprovePurchaseOrderStatusByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetPurchaseOrderByIdBackupInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(purchaseOrder == null, "Cannot find purchase order information");
            ThrowError.Against(purchaseOrder.StatusId != EnumPurchaseOrderStatus.Draft, "Can not modify purchase order in this status");

            purchaseOrder.StatusId = EnumPurchaseOrderStatus.Approved;
            purchaseOrder.LastSavedUser = loggedUser.AccountId.Value;

            _unitOfWork.PurchaseOrders.Update(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.PurchaseOrder,
                ActionType = EnumActionType.Approved,
                ObjectId = purchaseOrder.Id,
                ObjectName = purchaseOrder.Code
            });

            return true;
        }
    }
}

using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class ChangeStatusRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ChangeStatusRequestHandler : IRequestHandler<ChangeStatusRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public ChangeStatusRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(ChangeStatusRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            
            Product product = _unitOfWork.Products.GetProductByIdInStore(loggedUser.StoreId.Value, request.Id).FirstOrDefault();
            product.StatusId = product.StatusId == (int)EnumStatus.Active ? (int)EnumStatus.Inactive : (int)EnumStatus.Active;
            EnumActionType enumActionType = product.StatusId == (int)EnumStatus.Active ? EnumActionType.Inactivated : EnumActionType.Activated;

            await _unitOfWork.Products.UpdateAsync(product);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Product,
                ActionType = enumActionType,
                ObjectId = product.Id,
                ObjectName = product.Code.ToString(),
                ObjectThumbnail = product.Thumbnail
            });

            return true;
        }
    }
}

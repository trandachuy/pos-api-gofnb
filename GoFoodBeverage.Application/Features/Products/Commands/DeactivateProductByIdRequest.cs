using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class DeactivateProductByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateProductByIdRequestHandler : IRequestHandler<DeactivateProductByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeactivateProductByIdRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeactivateProductByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var product = await _unitOfWork
                .Products.Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.Id)
                .FirstOrDefaultAsync();

            try
            {
                product.IsActive = false;
                await _unitOfWork.Products.UpdateAsync(product);

                await _mediator.Send(new CreateStaffActivitiesRequest()
                {
                    ActionGroup = EnumActionGroup.Product,
                    ActionType = EnumActionType.Deleted,
                    ObjectId = product.Id,
                    ObjectName = product.Code.ToString(),
                    ObjectThumbnail = product.Thumbnail
                });
            }
            catch (Exception ex)
            {
                return false;
            }
            

            return true;
        }
    }
}

using GoFoodBeverage.Application.Features.Staffs.Commands;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Commands
{
    public class DeleteStoreBranchByIdRequest : IRequest<bool>
    {
        public Guid BranchId { get; set; }
    }

    public class DeleteStoreBranchByIdRequestHandler : IRequestHandler<DeleteStoreBranchByIdRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteStoreBranchByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMediator mediator
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DeleteStoreBranchByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var storeBranch = await _unitOfWork.StoreBranches
                .Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.BranchId && !m.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.Against(storeBranch == null, "Branch is not found");
            storeBranch.IsDeleted = true;
            await _unitOfWork.StoreBranches.UpdateAsync(storeBranch);

           // Insert activity history
           await _mediator.Send(new CreateStaffActivitiesRequest()
           {
               ActionGroup = EnumActionGroup.StoreBranch,
               ActionType = EnumActionType.Deleted,
               ObjectId = storeBranch.Id,
               ObjectName = storeBranch.Name.ToString(),
               ObjectThumbnail = storeBranch.Name
           });
            return true;
        }
    }

}

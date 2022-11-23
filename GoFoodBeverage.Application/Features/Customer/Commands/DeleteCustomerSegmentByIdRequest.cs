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
    public class DeleteCustomerSegmentByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCustomerSegmentRequestHandler : IRequestHandler<DeleteCustomerSegmentByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public DeleteCustomerSegmentRequestHandler(
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

        public async Task<bool> Handle(DeleteCustomerSegmentByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var customerSegment = await _unitOfWork.CustomerSegments.Find(c => c.StoreId == loggedUser.StoreId && c.Id == request.Id)
                .Include(c => c.CustomerCustomerSegments)
                .Include(o => o.CustomerSegmentConditions)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (customerSegment == null)
            {
                return false;
            }

            await _unitOfWork.CustomerSegments.RemoveAsync(customerSegment);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.CustomerSegment,
                ActionType = EnumActionType.Deleted,
                ObjectId = customerSegment.Id,
                ObjectName = customerSegment.Name
            });

            return true;
        }
    }
}

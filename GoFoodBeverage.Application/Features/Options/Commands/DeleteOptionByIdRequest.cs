using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Options.Commands
{
    public class DeleteOptionByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteOptionRequestHandler : IRequestHandler<DeleteOptionByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public DeleteOptionRequestHandler(
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

        public async Task<bool> Handle(DeleteOptionByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var option = await _unitOfWork.Options.Find(o => o.StoreId == loggedUser.StoreId && o.Id == request.Id)
                .Include(o => o.OptionLevel)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (option == null)
            {
                return false;
            }

            await _unitOfWork.Options.RemoveAsync(option);

            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Option,
                ActionType = EnumActionType.Deleted,
                ObjectId = option.Id,
                ObjectName = option.Name.ToString()
            });

            return true;
        }
    }
}

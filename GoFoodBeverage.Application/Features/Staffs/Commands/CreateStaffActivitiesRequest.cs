using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Staffs.Commands
{
    public class CreateStaffActivitiesRequest : IRequest<bool>
    {
        public EnumActionGroup ActionGroup { get; set; }

        public EnumActionType ActionType { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string ObjectThumbnail { get; set; }
    }

    public class CreateStaffActivitiesRequestHandler : IRequestHandler<CreateStaffActivitiesRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public CreateStaffActivitiesRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(CreateStaffActivitiesRequest request, CancellationToken cancellationToken)
        {

            try
            {
                var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

                if (!loggedUser.StoreId.HasValue || !loggedUser.Id.HasValue)
                    return false;

                StaffActivity entityCreate = new StaffActivity()
                {
                    StaffId = loggedUser.Id.Value,
                    StoreId = loggedUser.StoreId.Value,
                    ActionGroup = request.ActionGroup,
                    ActionType = request.ActionType,
                    ExecutedTime = DateTime.UtcNow,
                    ObjectId = request.ObjectId,
                    ObjectName = request.ObjectName,
                    ObjectThumbnail = request.ObjectThumbnail
                };
                await _unitOfWork.StaffActivities.AddAsync(entityCreate);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class DeleteMaterialManagementRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteMaterialManagementRequestHandler : IRequestHandler<DeleteMaterialManagementRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteMaterialManagementRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteMaterialManagementRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var material = await _unitOfWork.Materials
                .Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(material == null, "Material is not found");

            //Delete MaterialInventoryBranch
            var materialInventoryBranch = await _unitOfWork.MaterialInventoryBranches
                .Find(mib => mib.StoreId == loggedUser.StoreId && mib.MaterialId == request.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            await _unitOfWork.MaterialInventoryBranches.RemoveRangeAsync(materialInventoryBranch);

            //Delete UnitConversion
            var materialUnitConversion = _unitOfWork.UnitConversions.GetUnitConversionsByMaterialIdInStore(request.Id, loggedUser.StoreId);
            await _unitOfWork.UnitConversions.RemoveRangeAsync(materialUnitConversion);

            await _unitOfWork.Materials.RemoveAsync(material);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Material,
                ActionType = EnumActionType.Deleted,
                ObjectId = material.Id,
                ObjectName = material.Name.ToString(),
                ObjectThumbnail = material.Thumbnail
            });

            return true;
        }
    }
}

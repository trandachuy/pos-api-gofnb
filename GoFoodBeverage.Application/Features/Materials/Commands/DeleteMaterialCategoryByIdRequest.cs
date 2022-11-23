using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class DeleteMaterialCategoryByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteMaterialCategoryByIdRequestHandler : IRequestHandler<DeleteMaterialCategoryByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteMaterialCategoryByIdRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteMaterialCategoryByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialCategory = await _unitOfWork.MaterialCategories.GetMaterialCategoryByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);
            ThrowError.Against(materialCategory == null, "Material category is not found");

            _unitOfWork.MaterialCategories.Remove(materialCategory);
            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.MaterialCategory,
                ActionType = EnumActionType.Deleted,
                ObjectId = materialCategory.Id,
                ObjectName = materialCategory.Name.ToString()
            });

            return true;
        }
    }
}

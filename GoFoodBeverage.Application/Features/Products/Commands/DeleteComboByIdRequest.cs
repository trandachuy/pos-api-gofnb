using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class DeleteComboByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteComboRequestHandler : IRequestHandler<DeleteComboByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public DeleteComboRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<bool> Handle(DeleteComboByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var combo = _unitOfWork.Combos.GetAllCombosInStore(loggedUser.StoreId.Value)
               .Include(p => p.ComboStoreBranches)
               .Include(p => p.ComboProductGroups).ThenInclude(cpg => cpg.ComboProductGroupProductPrices)
               .Include(p => p.ComboProductPrices)
               .Include(p => p.ComboPricings).ThenInclude(cp => cp.ComboPricingProducts)
               .FirstOrDefault(x => x.Id == request.Id);

            ThrowError.Against(combo == null, "Combo is not found");

            await _unitOfWork.Combos.RemoveAsync(combo);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.Combo,
                ActionType = EnumActionType.Deleted,
                ObjectId = combo.Id,
                ObjectName = combo.Name.ToString(),
                ObjectThumbnail = combo.Thumbnail
            });

            return true;
        }
    }
}

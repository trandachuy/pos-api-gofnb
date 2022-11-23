using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Application.Features.Staffs.Commands;

namespace GoFoodBeverage.Application.Features.Products.Commands
{
    public class DeleteProductCategoryByIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCategoryRequestHandler : IRequestHandler<DeleteProductCategoryByIdRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public DeleteProductCategoryRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(DeleteProductCategoryByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var productCategory = await _unitOfWork.ProductCategories.Find(p => p.StoreId == loggedUser.StoreId && p.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(productCategory == null, "Product category is not found");
            await _unitOfWork.ProductCategories.RemoveAsync(productCategory);
            
            await _userActivityService.LogAsync(request);

            await _mediator.Send(new CreateStaffActivitiesRequest()
            {
                ActionGroup = EnumActionGroup.ProductCategory,
                ActionType = EnumActionType.Deleted,
                ObjectId = productCategory.Id,
                ObjectName = productCategory.Name.ToString()
            });

            return true;
        }
    }
}

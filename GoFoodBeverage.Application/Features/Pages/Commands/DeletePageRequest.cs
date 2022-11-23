using GoFoodBeverage.Application.Features.OnlineStoreMenus.Queries;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Pages.Commands
{
    public class DeletePageRequest : IRequest<DeletePageResponse>
    {
        public Guid PageId { get; set; }
    }

    public class DeletePageResponse : ResponseCommonModel { }

    public class DeletePageRequestHandler : IRequestHandler<DeletePageRequest, DeletePageResponse>
    {
        private readonly IMediator _mediator;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserProvider _userProvider;

        public DeletePageRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<DeletePageResponse> Handle(DeletePageRequest request, CancellationToken cancellationToken)
        {
            DeletePageResponse response = new DeletePageResponse();
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            if (!loggedUser.StoreId.HasValue)
            {
                response.IsSuccess = false;
                response.Message = "messages.userIsNotLogin";

                return response;
            }

            var menuItemsDependence = await _mediator.Send(new GetMenuItemReferenceToPageByPageIdRequest() { PageId = request.PageId });
            if (menuItemsDependence.Count() > 0)
            {
                response.IsSuccess = false;
                response.Message = "messages.pageDeletedNotSuccessfully";

                return response;
            }
            else
            {
                Page pageEntity = await _unitOfWork.Pages.Where(page => page.StoreId == loggedUser.StoreId.Value && page.Id == request.PageId).SingleOrDefaultAsync();
                if (pageEntity == null)
                {
                    response.IsSuccess = false;
                    response.Message = "messages.pageNotExist";

                    return response;
                }

                pageEntity.IsActive = true;
                await _unitOfWork.Pages.UpdateAsync(pageEntity);
            }

            response.IsSuccess = true;
            return response;
        }
    }
}

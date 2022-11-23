using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Page;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Pages.Commands
{
    public class UpdatePageRequest : IRequest<bool>
    {
        public PageModel Page { get; set; }
    }

    public class UpdatePageRequestHandler : IRequestHandler<UpdatePageRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdatePageRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var page = await _unitOfWork.Pages
                .GetAllPagesInStore(loggedUser.StoreId.Value)
                .FirstOrDefaultAsync(page => page.Id == request.Page.Id);

            ThrowError.BadRequestAgainstNull(page, "Cannot find page information");

            var previousData = page.ToJson();
            page.PageName = request.Page.PageName;
            page.PageContent = request.Page.PageContent;

            _unitOfWork.Pages.Update(page);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(previousData, page.ToJson());

            return true;
        }
    }
}
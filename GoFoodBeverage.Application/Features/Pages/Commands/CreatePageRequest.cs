using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Page;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Pages.Commands
{
    public class CreatePageRequest : IRequest<bool>
    {
        public PageModel Page { get; set; }
    }

    public class CreatePageRequestHandler : IRequestHandler<CreatePageRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreatePageRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreatePageRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var newPage = new Page()
            {
                StoreId = loggedUser.StoreId.Value,
                PageName = request.Page.PageName,
                PageContent = request.Page.PageContent,
                IsActive = true
            };

            _unitOfWork.Pages.Add(newPage);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }
    }
}

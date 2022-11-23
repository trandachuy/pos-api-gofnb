using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class UpdateAreaRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid StoreBranchId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateAreaRequestHandler : IRequestHandler<UpdateAreaRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateAreaRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateAreaRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);
            var areaNameExisted = await _unitOfWork.Areas
               .GetAreasByStoreBranchId(loggerUser.StoreId.Value, request.StoreBranchId)
               .FirstOrDefaultAsync(s => s.Name.ToLower() == request.Name.ToLower() && request.Id != s.Id);
            ThrowError.Against(areaNameExisted != null, "Duplicated");

            var currentArea = await _unitOfWork.Areas.GetAreaById(request.Id, loggerUser.StoreId.Value, request.StoreBranchId).FirstOrDefaultAsync();
            var updatedArea = UpdateArea(currentArea, request, loggerUser.AccountId.Value);

            await _unitOfWork.Areas.UpdateAsync(updatedArea);
            await _userActivityService.LogAsync(request);

            return true;
        }

        private static Area UpdateArea(Area updateArea, UpdateAreaRequest request, Guid accountId)
        {
            updateArea.Name = request.Name;
            updateArea.Description = request.Description;
            updateArea.IsActive = request.IsActive;
            updateArea.LastSavedUser = accountId;

            return updateArea;
        }
    }
}

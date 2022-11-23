using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class CreateAreaRequest : IRequest<bool>
    {
        public Guid StoreBranchId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class CreateAreaRequestHandler : IRequestHandler<CreateAreaRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public CreateAreaRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateAreaRequest request, CancellationToken cancellationToken)
        {
            var loggerUser = await _userProvider.ProvideAsync(cancellationToken);

            await CheckUniqueAndValidationAsync(request, loggerUser.StoreId.Value);

            var area = CreateArea(request, loggerUser.StoreId.Value, loggerUser.AccountId.Value);

            await _unitOfWork.Areas.AddAsync(area);

            await _userActivityService.LogAsync(request);

            return true;
        }

        private async Task CheckUniqueAndValidationAsync(CreateAreaRequest request, Guid storeId)
        {
            ThrowError.Against(string.IsNullOrEmpty(request.Name), new JObject()
            {
                { $"{nameof(request.Name)}", "Please enter area name" },
            });

            var areaNameExisted = await _unitOfWork.Areas
                .GetAreasByStoreBranchId(storeId, request.StoreBranchId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Name.ToLower() == request.Name.ToLower());

            ThrowError.Against(areaNameExisted != null, new JObject()
            {
                { $"{nameof(request.Name)}", "This area name is already existed" },
            });
        }

        private static Area CreateArea(CreateAreaRequest request, Guid storeId, Guid accountId)
        {
            var newArea = new Area()
            {
                Name = request.Name,
                Description = request.Description,
                StoreId = storeId,
                IsActive = true,
                StoreBranchId = request.StoreBranchId,
                CreatedUser = accountId
            };

            return newArea;
        }
    }
}

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using System.Linq;
using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetAvailableBranchQuantityRequest : IRequest<GetAvailableBranchQuantityResponse>
    {
    }

    public class GetAvailableBranchQuantityResponse
    {
        public int AvailableBranchQuantity { get; set; }
    }

    public class GetAvailableBranchNumberRequestHandler : IRequestHandler<GetAvailableBranchQuantityRequest, GetAvailableBranchQuantityResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;

        public GetAvailableBranchNumberRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
        }

        public async Task<GetAvailableBranchQuantityResponse> Handle(GetAvailableBranchQuantityRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdWithoutTrackingAsync(loggedUser.StoreId);
            var today = _dateTimeService.NowUtc;

            /// Get all order package of the store has approved
            var orderPackages = await _unitOfWork.OrderPackages
              .GetAll()
              .AsNoTracking()
              .Where(op => op.StoreId == loggedUser.StoreId &&
                           op.Status == EnumOrderPackageStatus.APPROVED.GetName() &&
                           op.IsActivated == true &&
                           op.ExpiredDate >= today)
              .Include(op => op.Package)
              .Select(op => new {
                  op.OrderPackageType,
                  op.Package,
                  op.BranchQuantity
              })
              .ToListAsync(cancellationToken: cancellationToken);

            var activatePackage = orderPackages.FirstOrDefault(op => op.OrderPackageType == EnumOrderPackageType.StoreActivate);
            if (activatePackage == null)
            {
                store.IsActivated = false;
                _unitOfWork.Stores.Update(store);
                await _unitOfWork.SaveChangesAsync();

                throw new Exception("Can not find the active store package. Please active your store to continue.");
            }

            var orderBranchPurchasePackages = orderPackages.Where(op => op.OrderPackageType == EnumOrderPackageType.BranchPurchase);
            var totalBranchPurchaseNumber = orderBranchPurchasePackages.Sum(o => o.BranchQuantity);
            var totalAvailableBranchNumber = activatePackage.Package.AvailableBranchNumber + totalBranchPurchaseNumber;
            var totalBranches = await _unitOfWork.StoreBranches
                .GetAll()
                .Where(b => b.StoreId == store.Id && !b.IsDeleted)
                .AsNoTracking()
                .CountAsync(cancellationToken: cancellationToken);

            var remainingAvailableBranchQuantity = totalAvailableBranchNumber - totalBranches;
            var response = new GetAvailableBranchQuantityResponse()
            {
                AvailableBranchQuantity = remainingAvailableBranchQuantity < 0 ? 0 : remainingAvailableBranchQuantity
            };

            return response;
        }
    }
}

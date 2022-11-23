using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using System.Linq;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class CheckBranchExpirationRequest : IRequest<bool>
    {
    }

    public class CheckBranchExpirationResponse
    {
    }

    public class CheckBranchExpirationRequestHandler : IRequestHandler<CheckBranchExpirationRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;

        public CheckBranchExpirationRequestHandler(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
        }

        public async Task<bool> Handle(CheckBranchExpirationRequest request, CancellationToken cancellationToken)
        {
            var today = _dateTimeService.NowUtc;
            var branchesExpiredDate = await _unitOfWork.StoreBranches
                .GetAll()
                .Where(b => b.StatusId == (int)EnumStatus.Active && b.ExpiredDate.HasValue && b.ExpiredDate < today && !b.IsDeleted)
                .ToListAsync(cancellationToken: cancellationToken);

            if(branchesExpiredDate.Any())
            {
                branchesExpiredDate.ForEach(branch =>
                {
                    branch.StatusId = (int)EnumStatus.Inactive;
                });

                _unitOfWork.StoreBranches.UpdateRange(branchesExpiredDate);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }
    }
}

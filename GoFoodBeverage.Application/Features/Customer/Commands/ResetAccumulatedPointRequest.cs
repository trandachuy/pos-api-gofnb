using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Commands
{
    public class ResetAccumulatedPointRequest : IRequest<bool>
    {
    }

    public class ResetAccumulatedPointHandler : IRequestHandler<ResetAccumulatedPointRequest, bool>
    {
        private IUnitOfWork _unitOfWork;

        public ResetAccumulatedPointHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ResetAccumulatedPointRequest request, CancellationToken cancellationToken)
        {
            var listPointConfig = await _unitOfWork.LoyaltyPointsConfigs.Find(x => x.IsActivated == true).ToListAsync();
            var listCustomerPointUpdates = new List<Domain.Entities.CustomerPoint>();
            var listStoreIds = listPointConfig.Select(x => x.StoreId);
            var listCustomerPointAll = await _unitOfWork.Customers.Find(x => listStoreIds.Contains(x.StoreId)).Include(x => x.CustomerPoint).Select(x => new { x.StoreId, x.CustomerPoint }).ToListAsync();

            foreach (var pointConfig in listPointConfig)
            {
                var resetAccumulatedPoint = false;
                var resetAvailablePoint = false;

                if (pointConfig.IsExpiryDate)
                {
                    if (pointConfig.ExpiryDate < DateTime.UtcNow)
                    {
                        resetAvailablePoint = true;
                    }
                }

                if (pointConfig.IsExpiryMembershipDate)
                {
                    if (pointConfig.ExpiryMembershipDate < DateTime.UtcNow)
                    {
                        resetAccumulatedPoint = true;
                    }
                }

                if (resetAccumulatedPoint || resetAvailablePoint)
                {
                    var listCustomerPoint = listCustomerPointAll.Where(x => x.StoreId == pointConfig.StoreId).Select(x => x.CustomerPoint);

                    if (resetAccumulatedPoint)
                    {
                        var listAccumulatedPoint = listCustomerPoint.Where(x => x.AccumulatedPoint > 0).ToList();
                        foreach (var customerPoint in listAccumulatedPoint)
                        {
                            customerPoint.AccumulatedPoint = 0;
                        }
                        listCustomerPointUpdates.AddRange(listAccumulatedPoint);
                    }
                    if (resetAvailablePoint)
                    {
                        var listAvailablePoint = listCustomerPoint.Where(x => x.AvailablePoint > 0).ToList();
                        foreach (var customerPoint in listAvailablePoint)
                        {
                            customerPoint.AvailablePoint = 0;
                        }
                        listCustomerPointUpdates.AddRange(listAvailablePoint);
                    }
                }
            }

            if (listCustomerPointUpdates.Count > 0)
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.CustomerPoints.UpdateRangeAsync(listCustomerPointUpdates);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            return true;
        }
    }
}

using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Unit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Units.Commands
{
    public class UpdateUnitConversionsRequest : IRequest<bool>
    {
        public IEnumerable<UpdateUnitConversionDto> UnitConversions { get; set; }
    }

    public class UpdateUnitConversionsRequestHandler : IRequestHandler<UpdateUnitConversionsRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public UpdateUnitConversionsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateUnitConversionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            /// Update UnitConversions from request
            var unitConversion = await _unitOfWork.UnitConversions
                .Find(u => u.StoreId == loggedUser.StoreId && u.MaterialId == request.UnitConversions.FirstOrDefault().MaterialId)
                .ToListAsync(cancellationToken: cancellationToken);

            var updateUnitConversions = await UpdateUnitConversions(request, unitConversion, loggedUser.StoreId, loggedUser.AccountId);
            _unitOfWork.UnitConversions.UpdateRange(updateUnitConversions);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(UpdateUnitConversionsRequest request)
        {
            ThrowError.Against(request.UnitConversions.Count() == 0, "Please enter unit conversion");
        }

        private async Task<List<UnitConversion>> UpdateUnitConversions(UpdateUnitConversionsRequest request, List<UnitConversion> oldUnitConversions, Guid? storeId, Guid? accountId)
        {
            var newUnitConversions = new List<UnitConversion>();
            var currentUnitConversions = request.UnitConversions.ToList();

            var deleteUnitConversions = oldUnitConversions
                .Where(x => !currentUnitConversions.Where(x => x.Id != Guid.Empty).Any(y => y.Id == x.Id));

            currentUnitConversions.ForEach(current =>
            {
                if (current.Id == Guid.Empty)
                {
                    var unitConversion = new UnitConversion()
                    {
                        StoreId = storeId,
                        UnitId = current.UnitId,
                        Capacity = current.Capacity,
                        MaterialId = current.MaterialId,
                        LastSavedUser = accountId,
                        CreatedUser = accountId
                    };
                    newUnitConversions.Add(unitConversion);
                }
                else
                {
                    var unitConversion = new UnitConversion()
                    {
                        Id = current.Id,
                        StoreId = storeId,
                        UnitId = current.UnitId,
                        Capacity = current.Capacity,
                        MaterialId = current.MaterialId,
                        LastSavedUser = accountId,
                        CreatedUser = accountId
                    };
                    _unitOfWork.UnitConversions.UpdateAsync(unitConversion);
                }
            });

            await _unitOfWork.UnitConversions.AddRangeAsync(newUnitConversions);
            await _unitOfWork.UnitConversions.RemoveRangeAsync(deleteUnitConversions);

            return newUnitConversions;
        }
    }
}

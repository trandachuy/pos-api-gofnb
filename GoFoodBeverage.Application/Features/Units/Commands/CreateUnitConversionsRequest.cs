using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Unit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Units.Commands
{
    public class CreateUnitConversionsRequest : IRequest<bool>
    {
        public IEnumerable<CreateUnitConversionDto> UnitConversions { get; set; }
    }

    public class CreateUnitConversionsRequestHandler : IRequestHandler<CreateUnitConversionsRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateUnitConversionsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateUnitConversionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            RequestValidation(request);

            /// Check existence
            var unitIds = request.UnitConversions.Select(u => u.UnitId).ToList();
            var unitConversionsExisted = await _unitOfWork.UnitConversions
                .GetAllUnitConversionsInStore(loggedUser.StoreId.Value)
                .Where(u => unitIds.Any(uid => uid == u.UnitId))
                .CountAsync(cancellationToken: cancellationToken);
            ThrowError.Against(unitConversionsExisted > 0, "Some unit conversions has been existed capacity conversion");

            /// Insert UnitConversions from request
            var newUnitConversions = CreateUnitConversions(request, loggedUser.StoreId);
            _unitOfWork.UnitConversions.AddRange(newUnitConversions);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(CreateUnitConversionsRequest request)
        {
            ThrowError.Against(request.UnitConversions.Count() == 0, "Please enter unit conversion");
        }

        private static List<UnitConversion> CreateUnitConversions(CreateUnitConversionsRequest request, Guid? storeId)
        {
            List<UnitConversion> unitConversions = new();
            foreach (var unitConversion in request.UnitConversions)
            {
                var newUnitConversion = new UnitConversion()
                {
                    UnitId = unitConversion.UnitId,
                    Capacity = unitConversion.Capacity,
                    StoreId = storeId
                };
                unitConversions.Add(newUnitConversion);
            }

            return unitConversions;
        }
    }
}

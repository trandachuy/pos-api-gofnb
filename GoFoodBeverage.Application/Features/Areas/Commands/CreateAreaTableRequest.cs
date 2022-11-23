using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class CreateAreaTableRequest : IRequest<bool>
    {
        public Guid AreaId { get; set; }

        public List<AreaTableDto> AreaTables { get; set; }

        public class AreaTableDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int NumberOfSeat { get; set; }
        }
    }

    public class CreateAreaTableRequestHandler : IRequestHandler<CreateAreaTableRequest, bool>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserActivityService _userActivityService;

        public CreateAreaTableRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IUserActivityService userActivityService
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(CreateAreaTableRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            var area = await _unitOfWork.Areas
                .GetAllAreasInStore(loggedUser.StoreId)
                .FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken: cancellationToken);
            ThrowError.Against(area == null, "Cannot find area information");

            RequestValidation(request);

            var newAreatables = CreateAreatable(request, loggedUser.AccountId.Value, loggedUser.StoreId.Value);
            await _unitOfWork.AreaTables.AddRangeAsync(newAreatables);
            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(CreateAreaTableRequest request)
        {
            ThrowError.Against(request.AreaTables.Count == 0, "Please enter table.");
        }

        private static List<AreaTable> CreateAreatable(CreateAreaTableRequest request, Guid accountId, Guid? storeId)
        {
            var areaTables = new List<AreaTable>();
            request.AreaTables.ForEach(p =>
            {
                var areaTable = new AreaTable()
                {
                    AreaId = request.AreaId,
                    Name = p.Name,
                    NumberOfSeat = p.NumberOfSeat,
                    IsActive = true,
                    CreatedUser = accountId,
                    StoreId = storeId,
                };
                areaTables.Add(areaTable);
            });

            return areaTables;
        }
    }
}

using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Areas.Commands
{
    public class UpdateAreaTableRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid StoreBranchId { get; set; }

        public Guid AreaId { get; set; }

        public bool IsActive { get; set; }

        public List<AreaTableDto> Tables { get; set; }

        public class AreaTableDto
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }

            public int NumberOfSeat { get; set; }
        }
    }

    public class UpdateAreaTableRequestHandler : IRequestHandler<UpdateAreaTableRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateAreaTableRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateAreaTableRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var areaTable = await _unitOfWork.AreaTables
                .GetAreaTableByIdAsync(request.Id, loggedUser.StoreId);

            ThrowError.Against(areaTable == null, "Cannot find area table information");

            RequestValidation(request);

            var modifiedTable = await UpdateAreaTableAsync(areaTable, request, loggedUser.StoreId.Value);

            await _unitOfWork.AreaTables.UpdateAsync(modifiedTable);

            await _unitOfWork.SaveChangesAsync();

            await _userActivityService.LogAsync(request);

            return true;
        }

        private static void RequestValidation(UpdateAreaTableRequest request)
        {
            ThrowError.Against(request.Tables.Count == 0, "Please enter table.");
            ThrowError.Against(IsAnyTableNameDuplicated(request.Tables) == true, "Table name is duplicated");
        }

        private static bool IsAnyTableNameDuplicated(List<UpdateAreaTableRequest.AreaTableDto> tables)
        {
            foreach (var table in tables)
            {
                if (IsDuplicatedTableName(tables, table)) return true;
            }

            return false;
        }

        private static bool IsDuplicatedTableName(List<UpdateAreaTableRequest.AreaTableDto> tables, UpdateAreaTableRequest.AreaTableDto table)
        {
            var count = tables.Count(tb => tb.Name.ToLower().Trim().Equals(table.Name.ToLower().Trim()));
            return count > 1;
        }

        public async Task<AreaTable> UpdateAreaTableAsync(AreaTable currentAreaTable, UpdateAreaTableRequest request, Guid? storeId)
        {
            #region Update tables
            var newTables = new List<AreaTable>();

            if (request.Tables != null && request.Tables.Any())
            {
                foreach (var table in request.Tables)
                {
                    if (table.Id == null)
                    {
                        var newTable = new AreaTable()
                        {
                            Name = table.Name,
                            NumberOfSeat = table.NumberOfSeat,
                            AreaId = request.AreaId,
                            IsActive = request.IsActive,
                            StoreId = storeId
                        };
                        newTables.Add(newTable);
                    }
                    else
                    {
                        currentAreaTable.Name = table.Name;
                        currentAreaTable.NumberOfSeat = table.NumberOfSeat;
                        currentAreaTable.AreaId = request.AreaId;
                        currentAreaTable.IsActive = request.IsActive;
                    }
                }
                _unitOfWork.AreaTables.AddRange(newTables);

                await _unitOfWork.SaveChangesAsync();
            }
            #endregion

            return currentAreaTable;
        }
    }
}

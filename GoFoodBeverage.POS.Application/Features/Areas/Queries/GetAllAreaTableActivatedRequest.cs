using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Area;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Areas.Queries
{
    public class GetAllAreasActivatedRequest : IRequest<GetActiveAreaTableResponse>
    {
    }

    public class GetActiveAreaTableResponse
    {
        public IEnumerable<AreaTablesByBranchIdModel> Areas { get; set; }
    }

    public class GetAllAreasActivatedRequestHandler : IRequestHandler<GetAllAreasActivatedRequest, GetActiveAreaTableResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetAllAreasActivatedRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetActiveAreaTableResponse> Handle(GetAllAreasActivatedRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var data = await _unitOfWork.Areas
                .POS_GetActiveAreasByStoreBranchId(loggedUser.StoreId, loggedUser.BranchId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            var areas = _mapper.Map<List<AreaTablesByBranchIdModel>>(data);
            areas.ForEach(item =>
            {
                item.AreaTables.ToList().ForEach(areaTable =>
                {
                    areaTable.Orders = areaTable.Orders.Where(order => order.CreatedTime.HasValue && order.CreatedTime.Value.Date >= DateTime.UtcNow.Date);
                    if (areaTable.Orders.Any())
                    {
                        var createdTime = areaTable.Orders.FirstOrDefault().CreatedTime.GetValueOrDefault();
                        areaTable.NumberOfStep = string.Format("{0}/{1}", areaTable.Orders.Count(w => (int)w.StatusId == (int)EnumOrderStatus.Processing), areaTable.Orders.Count());
                        areaTable.Time = $"{createdTime.Hour}:{createdTime.Minute}:{createdTime.Second}";
                        areaTable.OrderCreateDate = createdTime;
                        areaTable.PriceAfterDiscount = areaTable.Orders.Sum(s => s.OriginalPrice - s.TotalDiscountAmount);
                    }
                    else
                    {
                        areaTable.NumberOfStep = "-";
                        areaTable.Time = "-";
                        areaTable.PriceAfterDiscount = 0;
                    }

                });
                item.AreaTables = item.AreaTables.OrderBy(o => o.Name);
            });
            var response = new GetActiveAreaTableResponse()
            {
                Areas = areas
            };

            return response;
        }
    }
}

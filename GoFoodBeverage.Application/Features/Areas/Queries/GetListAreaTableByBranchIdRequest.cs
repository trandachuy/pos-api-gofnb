using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Area;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using GoFoodBeverage.Interfaces;
using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using MediatR;

namespace GoFoodBeverage.Application.Features.Areas.Queries
{
    public class GetListAreaTableByBranchIdRequest : IRequest<GetListAreaTableByBranchIdResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }

        public Guid? StoreBranchId { get; set; }
    }

    public class GetListAreaTableByBranchIdResponse
    {
        public IEnumerable<AreaTableModel> AreaTables { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetListAreaTableByBranchIdRequestHandler : IRequestHandler<GetListAreaTableByBranchIdRequest, GetListAreaTableByBranchIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListAreaTableByBranchIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetListAreaTableByBranchIdResponse> Handle(GetListAreaTableByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var areaTables = _unitOfWork.AreaTables.GetAllAreaTablesByStoreBranchId(loggedUser.StoreId.Value, request.StoreBranchId);

            if (!string.IsNullOrEmpty(request.KeySearch))
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                areaTables = areaTables.Where(g => g.Name.ToLower().Contains(keySearch));
            }

            var allAreaTablesInStoreBranch = await areaTables
                .AsNoTracking()
                .Include(t => t.Area)
                .ThenInclude(a => a.StoreBranch)
                .OrderBy(x => x.Area.Name)
                .ThenByDescending(x => x.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);

            var pagingResult = allAreaTablesInStoreBranch.Result;
            var areaTableListResponse = _mapper.Map<List<AreaTableModel>>(pagingResult);
            areaTableListResponse.ForEach(t =>
            {
                var areaTable = pagingResult.FirstOrDefault(i => i.Id == t.Id);
                var area = areaTable.Area;
                t.Area = _mapper.Map<AreaModel>(area);
                t.No = areaTableListResponse.IndexOf(t) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetListAreaTableByBranchIdResponse()
            {
                PageNumber = request.PageNumber,
                Total = allAreaTablesInStoreBranch.Total,
                AreaTables = areaTableListResponse
            };

            return response;
        }
    }
}

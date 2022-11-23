using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Area;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Extensions;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Areas.Queries
{
    public class GetAreasByBranchIdRequest : IRequest<GetAreasByBranchIdResponse>
    {
        public Guid StoreBranchId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetAreasByBranchIdResponse
    {
        public IEnumerable<AreaModel> Areas { get; set; }

        public int Total { get; set; }
    }

    public class GetAreasByBranchIdRequestHandler : IRequestHandler<GetAreasByBranchIdRequest, GetAreasByBranchIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetAreasByBranchIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetAreasByBranchIdResponse> Handle(GetAreasByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var areasByPaging = await _unitOfWork.Areas
                .GetAreasByStoreBranchId(loggedUser.StoreId, request.StoreBranchId)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);

            var areaModels = _mapper.Map<IEnumerable<AreaModel>>(areasByPaging.Result);
            var response = new GetAreasByBranchIdResponse()
            {
                Areas = areaModels,
                Total = areasByPaging.Total
            };

            return response;
        }
    }
}

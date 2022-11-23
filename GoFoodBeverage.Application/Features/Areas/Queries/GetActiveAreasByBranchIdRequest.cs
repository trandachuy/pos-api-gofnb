using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Area;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Areas.Queries
{
    public class GetActiveAreasByBranchIdRequest : IRequest<GetActiveAreasByBranchIdResponse>
    {
        public Guid StoreBranchId { get; set; }
    }

    public class GetActiveAreasByBranchIdResponse
    {
        public IEnumerable<AreaTablesByBranchIdModel> Areas { get; set; }
    }

    public class GetActiveAreasByBranchIdRequestHandler : IRequestHandler<GetActiveAreasByBranchIdRequest, GetActiveAreasByBranchIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetActiveAreasByBranchIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetActiveAreasByBranchIdResponse> Handle(GetActiveAreasByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var areas = await _unitOfWork.Areas
                .GetActiveAreasByStoreBranchId(loggedUser.StoreId, request.StoreBranchId)
                .AsNoTracking()
                .ProjectTo<AreaTablesByBranchIdModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetActiveAreasByBranchIdResponse()
            {
                Areas = areas
            };

            return response;
        }
    }
}

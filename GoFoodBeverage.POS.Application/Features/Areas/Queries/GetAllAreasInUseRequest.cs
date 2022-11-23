using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Area;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Areas.Queries
{
    public class GetAllAreasInUseRequest : IRequest<GetAreaTableResponse>
    {
    }

    public class GetAreaTableResponse
    {
        public IEnumerable<AreaTablesByBranchIdModel> Areas { get; set; }
    }

    public class GetAreasByIdRequestHandler : IRequestHandler<GetAllAreasInUseRequest, GetAreaTableResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetAreasByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetAreaTableResponse> Handle(GetAllAreasInUseRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var data = await _unitOfWork.Areas
                .POS_GetAreasInUseByStoreBranchId(loggedUser.StoreId, loggedUser.BranchId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            var areas = _mapper.Map<List<AreaTablesByBranchIdModel>>(data);
            var response = new GetAreaTableResponse()
            {
                Areas = areas
            };

            return response;
        }
    }
}

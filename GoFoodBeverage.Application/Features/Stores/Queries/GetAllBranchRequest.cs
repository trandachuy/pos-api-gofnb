using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using System;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetAllBranchRequest : IRequest<GetAllBranchsResponse>
    {
        
    }

    public class GetAllBranchsResponse
    {
        public IEnumerable<StoreBranchModel> Branchs { get; set; }
    }

    public class GetAllBranchRequestHandler : IRequestHandler<GetAllBranchRequest, GetAllBranchsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IBranchService _branchService;

        public GetAllBranchRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IBranchService branchService )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _branchService = branchService;
        }

        public async Task<GetAllBranchsResponse> Handle(GetAllBranchRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            var branches = _branchService.GetBranches((Guid)loggedUser.StoreId, loggedUser.AccountId);
            var response = new GetAllBranchsResponse()
            {
                Branchs = _mapper.Map<List<StoreBranchModel>>(branches)
            };
            return response;
        }
    }
}

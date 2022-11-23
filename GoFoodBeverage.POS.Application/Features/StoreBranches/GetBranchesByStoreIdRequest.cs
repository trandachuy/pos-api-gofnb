using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Stores.Queries
{
    public class GetBranchesByStoreIdRequest : IRequest<GetBranchesByStoreIdResponse>
    {
        public Guid StoreId { get; set; }

        public Guid? AccountId { get; set; }
    }

    public class GetBranchesByStoreIdResponse
    {
        public IEnumerable<StoreBranchModel> StoreBranches { get; set; }
    }

    public class GetBranchesByStoreIdRequestHandler : IRequestHandler<GetBranchesByStoreIdRequest, GetBranchesByStoreIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMapper _mapper;

        public GetBranchesByStoreIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetBranchesByStoreIdResponse> Handle(GetBranchesByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var branchesResponse = new List<StoreBranchModel>();
            var stores = _unitOfWork.Stores.GetStoresByAccountId(request.AccountId).Where(x => x.Id == request.StoreId).ToList();
            if (!request.AccountId.HasValue || stores.Any())
            {
                branchesResponse = await _unitOfWork.StoreBranches
                    .GetStoreBranchesByStoreId(request.StoreId)
                    .AsNoTracking()
                    .ProjectTo<StoreBranchModel>(_mapperConfiguration)
                    .ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {

                var staff = await _unitOfWork.Staffs.GetStaffByAccountId(request.AccountId.Value).AsNoTracking().FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var staffGroupPermissionBranches = await _unitOfWork.StaffGroupPermissionBranches
                                                                    .GetStaffGroupPermissionBranchesByStaffId(staff.Id)
                                                                    .AsNoTracking()
                                                                    .ToListAsync();
                var allBranchesByStoreId = await _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(request.StoreId).AsNoTracking().ToListAsync();
                var branches = allBranchesByStoreId;
                var isNotApplyAllBranches = staffGroupPermissionBranches.SelectMany(s => s.GroupPermissionBranches).All(i => i.IsApplyAllBranch == false);
                if (isNotApplyAllBranches)
                {
                    branches = staffGroupPermissionBranches.SelectMany(s => s.GroupPermissionBranches.Where(g => g.IsApplyAllBranch == false).Select(g => g.StoreBranch)).DistinctBy(b => b.Id).ToList();
                }

                branchesResponse = _mapper.Map<List<StoreBranchModel>>(branches);
            }

            var response = new GetBranchesByStoreIdResponse()
            {
                StoreBranches = branchesResponse
            };

            return response;
        }
    }
}

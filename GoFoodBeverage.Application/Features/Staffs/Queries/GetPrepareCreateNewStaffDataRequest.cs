using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Staff;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Staffs.Queries
{
    public class GetPrepareCreateNewStaffDataRequest : IRequest<GetPrepareCreateNewStaffDataResponse>
    {

    }

    public class GetPrepareCreateNewStaffDataResponse
    {
        public IEnumerable<StaffGroupPermissionModel.GroupPermissionDto> GroupPermissions { get; set; }

        public IEnumerable<StaffGroupPermissionModel.BranchDto> Branches { get; set; }
    }

    public class GetPrepareCreateNewStaffDataRequestHandler : IRequestHandler<GetPrepareCreateNewStaffDataRequest, GetPrepareCreateNewStaffDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetPrepareCreateNewStaffDataRequestHandler(
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

        public async Task<GetPrepareCreateNewStaffDataResponse> Handle(GetPrepareCreateNewStaffDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var groupPermissions = await _unitOfWork.GroupPermissions
                .GetGroupPermissionsByStoreId(loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<StaffGroupPermissionModel.GroupPermissionDto>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var branches = await _unitOfWork.StoreBranches
               .GetStoreBranchesByStoreId(loggedUser.StoreId)
               .AsNoTracking()
               .ProjectTo<StaffGroupPermissionModel.BranchDto>(_mapperConfiguration)
               .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetPrepareCreateNewStaffDataResponse()
            {
                GroupPermissions = groupPermissions,
                Branches = branches
            };

            return response;
        }
    }
}

using MediatR;
using MoreLinq;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Permission;

namespace GoFoodBeverage.POS.Application.Features.Settings.Queries
{
    /// <summary>
    ///  Get permissions from all store branches
    /// </summary>
    public class GetPermissionsRequest : IRequest<GetPermissionsResponse>
    {
    }

    public class GetPermissionsResponse
    {
        public IEnumerable<PermissionModel> Permissions { get; set; }
    }

    public class GetPermissionsRequestHandler : IRequestHandler<GetPermissionsRequest, GetPermissionsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetPermissionsRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)

        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetPermissionsResponse> Handle(GetPermissionsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            /// If the store was set full permission in initial
            var isStaffInitStore = await _unitOfWork.Stores.IsStaffInitStoreAsync(loggedUser.AccountId.Value, loggedUser.StoreId.Value);
            if (isStaffInitStore)
            {
                var allPermissions = await _unitOfWork.Permissions
                    .GetAll()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                var permissions = _mapper.Map<IEnumerable<PermissionModel>>(allPermissions);
                return new GetPermissionsResponse()
                {
                    Permissions = permissions
                };
            }
            else
            {
                /// Get group permissions of staff in all branches 
                var staffGroupPermissionBranches = await _unitOfWork.StaffGroupPermissionBranches
                    .GetStaffGroupPermissionBranchesByStaffId(loggedUser.Id.Value)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);
                var groupPermissions = staffGroupPermissionBranches
                    .SelectMany(s => s.GroupPermissionBranches.Select(g => g.GroupPermission))
                    .DistinctBy(g => g.Id);

                var groupPermissionIds = groupPermissions.Select(g => g.Id);

                var groupPermissionPermissions = await _unitOfWork.GroupPermissionPermissions
                    .Find(g => groupPermissionIds.Any(gpid => gpid == g.GroupPermissionId))
                    .AsNoTracking()
                    .Include(g => g.Permission)
                    .ToListAsync(cancellationToken: cancellationToken);

                var permissions = groupPermissionPermissions.Select(g => g.Permission);
                var permissionsDisticted = permissions.DistinctBy(p => p.Id).ToList();
                var permissionsResponse = _mapper.Map<IEnumerable<PermissionModel>>(permissionsDisticted);

                return new GetPermissionsResponse()
                {
                    Permissions = permissionsResponse
                };
            }
        }
    }
}

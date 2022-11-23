using MediatR;
using MoreLinq;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Application.Features.Settings.Queries
{
    /// <summary>
    ///  Get permissions from all store branches
    /// </summary>
    public class GetPermissionsRequest : IRequest<GetPermissionsResponse>
    {
        public string Token { get; set; }
    }

    public class GetPermissionsResponse
    {
        public IEnumerable<PermissionModel> Permissions { get; set; }
        public IEnumerable<PermissionGroupResult> PermissionGroup { get; set; }
    }

    public class PermissionGroupResult
    {
        public string Name { get; set; }

        public Guid PermissionGroupId { get; set; }

        public bool IsFullPermission { get; set; }

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
            var loggedUser = _userProvider.GetLoggedUserModelFromJwt(request.Token);
            var permissionsResponse = new List<PermissionModel>();
            /// If is initial store set full permission
            var isStaffInitStore = await _unitOfWork.Stores.IsStaffInitStoreAsync(loggedUser.AccountId.Value, loggedUser.StoreId.Value);
            var allPermissions = await _unitOfWork.Permissions
                    .GetAll()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);
            if (isStaffInitStore)
            {
                
                permissionsResponse = _mapper.Map<List<PermissionModel>>(allPermissions);
                /// If the account is ADMIN, add admin permission
                permissionsResponse.Add(new PermissionModel()
                {
                    Id = EnumPermission.ADMIN.ToGuid(),
                    Name = EnumPermission.ADMIN.ToString()
                });
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
                /// Get permissions
                var groupPermissionPermissions = await _unitOfWork.GroupPermissionPermissions
                    .Find(g => groupPermissionIds.Any(gpid => gpid == g.GroupPermissionId))
                    .AsNoTracking()
                    .Include(g => g.Permission)
                    .ToListAsync(cancellationToken: cancellationToken);

                var permissions = groupPermissionPermissions.Select(g => g.Permission);
                var permissionsDisticted = permissions.DistinctBy(p => p.Id).ToList();
                permissionsResponse = _mapper.Map<List<PermissionModel>>(permissionsDisticted);
            }

            var allPermissionGroups = _unitOfWork.PermissionGroups.GetAll().ToList();
            var permissionGroup = allPermissionGroups.Select(x => new PermissionGroupResult
            {
                Name = x.Name,
                PermissionGroupId = x.Id,
                IsFullPermission = allPermissions.Count(c => c.PermissionGroupId == x.Id) == permissionsResponse.Count(c => c.PermissionGroupId == x.Id),
            }).ToList();

            return new GetPermissionsResponse()
            {
                Permissions = permissionsResponse,
                PermissionGroup = permissionGroup
            };
        }
    }
}

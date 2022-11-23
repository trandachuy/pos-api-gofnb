using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Permission;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services.User
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserPermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckPermissionForUserAsync(ClaimsPrincipal claimsPrincipal, EnumPermission requirementPermission)
        {
            try
            {
                #region Check internal tool permission
                if (requirementPermission == EnumPermission.INTERNAL_TOOL)
                {
                    var claimInternalAccountId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.INTERNAL_ACCOUNT_ID);
                    if (claimInternalAccountId == null) return false;

                    var internalAccountId = Guid.Parse(claimInternalAccountId.Value);
                    var internalAccount = _unitOfWork.InternalAccounts.Find(i => i.Id == internalAccountId).AsNoTracking().FirstOrDefault();
                        
                    return internalAccount != null;
                }
                #endregion

                var claimAccountId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
                var claimStoreId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.STORE_ID);
                var claimStaffId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ID);

                var accountId = Guid.Parse(claimAccountId.Value);
                var storeId = Guid.Parse(claimStoreId.Value);
                var staffId = Guid.Parse(claimStaffId.Value);

                var isStaffInitStore = _unitOfWork.Stores.IsStaffInitStore(accountId, storeId);
                var allPermissions = _unitOfWork.Permissions.GetAll().AsNoTracking();
                
                if(isStaffInitStore)
                {
                    // Get all permission from package and check
                    var allStorePermissions = await _unitOfWork.Permissions
                        .GetAll()
                        .Select(p => p.Id)
                        .ToListAsync();

                    allStorePermissions.Add(EnumPermission.ADMIN.ToGuid());

                    return HasPermission(allStorePermissions, requirementPermission.ToGuid());
                }

                // Get all permission assigned to user and check
                var groupPermissionIds = _unitOfWork
                    .StaffGroupPermissionBranches
                    .GetAll()
                    .AsNoTracking()
                    .Where(s => s.StaffId == staffId)
                    .Include(s => s.GroupPermissionBranches)
                    .SelectMany(s => s.GroupPermissionBranches
                    .Select(g => g.GroupPermissionId))
                    .Distinct();

                var permissions = _unitOfWork.GroupPermissionPermissions
                    .Find(g => groupPermissionIds.Any(gpid => gpid == g.GroupPermissionId))
                    .AsNoTracking()
                    .Include(g => g.Permission)
                    .Select(g => g.Permission);

                return HasPermission(permissions, requirementPermission);
            } 
            catch
            {

            }

            return false;
        }

        private static bool HasPermission(IQueryable<Permission> permissions, EnumPermission permission)
        {
            var existedPermission = permissions.FirstOrDefault(p => p.Id == permission.ToGuid());

            return existedPermission != null;
        }

        private static bool HasPermission(List<EnumPermission> permissionIds, EnumPermission permission)
        {
            var existedPermission = permissionIds.Any(pid => pid == permission);

            return existedPermission;
        }

        private static bool HasPermission(List<Guid> permissionIds, Guid permission)
        {
            var existedPermission = permissionIds.Any(pid => pid == permission);

            return existedPermission;
        }

    }
}

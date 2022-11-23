using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class GroupPermissionRepository : GenericRepository<GroupPermission>, IGroupPermissionRepository
    {
        public GroupPermissionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<GroupPermission> GetGroupPermissionsByStoreId(Guid? storeId)
        {
            var groupPermissions = dbSet.Where(groupPermission => groupPermission.StoreId == storeId);

            return groupPermissions;
        }

        public async Task<GroupPermission> GetGroupPermissionByIdInStoreAsync(Guid groupPermissionId, Guid storeId)
        {
            var permissionGroup = await dbSet
                .Where(m => m.Id == groupPermissionId && m.StoreId == storeId)
                .Include(p => p.GroupPermissionPermissions)
                .FirstOrDefaultAsync();

            return permissionGroup;
        }
    }
}

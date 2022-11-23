using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System.Linq;
using System;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Permission> GetPermissionsByPermissionGroupId(Guid permissionGroupId)
        {
            var permissions = dbSet.Where(permission => permission.PermissionGroupId == permissionGroupId);

            return permissions;
        }
    }
}

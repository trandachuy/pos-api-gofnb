using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class GroupPermissionPermissionRepository : GenericRepository<GroupPermissionPermission>, IGroupPermissionPermissionRepository
    {
        public GroupPermissionPermissionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public void DeleteByGroupPermissionId(Guid id)
        {
            var list = dbSet.Where(x => x.GroupPermissionId == id);
            dbSet.RemoveRange(list);
        }
    }
}

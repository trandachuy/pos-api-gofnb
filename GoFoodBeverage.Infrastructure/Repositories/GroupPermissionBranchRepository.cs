using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class GroupPermissionBranchRepository : GenericRepository<GroupPermissionBranch>, IGroupPermissionBranchRepository
    {
        public GroupPermissionBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

    }
}

using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PackageFunctionGroupRepository : GenericRepository<PackageFunction>, IPackageFunctionGroupRepository
    {
        public PackageFunctionGroupRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FunctionPermissionRepository : GenericRepository<FunctionPermission>, IFunctionPermissionRepository
    {
        public FunctionPermissionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

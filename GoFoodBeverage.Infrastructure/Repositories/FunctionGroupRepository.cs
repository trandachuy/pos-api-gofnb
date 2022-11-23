using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FunctionGroupRepository : GenericRepository<FunctionGroup>, IFunctionGroupRepository
    {
        public FunctionGroupRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

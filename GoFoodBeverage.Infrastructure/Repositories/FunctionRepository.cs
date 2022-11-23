using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FunctionRepository : GenericRepository<Function>, IFunctionRepository
    {
        public FunctionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

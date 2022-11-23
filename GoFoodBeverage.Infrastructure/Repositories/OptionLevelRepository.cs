using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OptionLevelRepository : GenericRepository<OptionLevel>, IOptionLevelRepository
    {
        public OptionLevelRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

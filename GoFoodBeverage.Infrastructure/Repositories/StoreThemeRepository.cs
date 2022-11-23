using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreThemeRepository : GenericRepository<StoreTheme>, IStoreThemeRepository
    {
        public StoreThemeRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

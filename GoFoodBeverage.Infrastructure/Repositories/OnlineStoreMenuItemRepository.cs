using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OnlineStoreMenuItemRepository : GenericRepository<OnlineStoreMenuItem>, IOnlineStoreMenuItemRepostiory
    {
        public OnlineStoreMenuItemRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

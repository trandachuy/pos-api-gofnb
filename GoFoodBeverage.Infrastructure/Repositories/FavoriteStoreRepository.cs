using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FavoriteStoreRepository : GenericRepository<FavoriteStore>, IFavoriteStoreRepository
    {
        public FavoriteStoreRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

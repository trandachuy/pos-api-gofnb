using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductPlatformRepository : GenericRepository<ProductPlatform>, IProductPlatformRepository
    {
        public ProductPlatformRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

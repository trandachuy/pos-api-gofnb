using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductChannelRepository : GenericRepository<ProductChannel>, IProductChannelRepository
    {
        public ProductChannelRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

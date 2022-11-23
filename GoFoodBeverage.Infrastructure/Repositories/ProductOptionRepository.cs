using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductOptionRepository : GenericRepository<ProductOption>, IProductOptionRepository
    {
        public ProductOptionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}

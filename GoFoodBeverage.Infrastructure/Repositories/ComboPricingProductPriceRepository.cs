using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboPricingProductPriceRepository : GenericRepository<ComboPricingProductPrice>, IComboPricingProductPriceRepository
    {
        public ComboPricingProductPriceRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

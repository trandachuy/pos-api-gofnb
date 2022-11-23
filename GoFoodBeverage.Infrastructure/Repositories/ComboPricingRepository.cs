using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboPricingRepository : GenericRepository<ComboPricing>, IComboPricingRepository
    {
        public ComboPricingRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

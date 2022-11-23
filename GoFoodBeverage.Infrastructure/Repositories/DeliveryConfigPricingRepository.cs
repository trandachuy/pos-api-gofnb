using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class DeliveryConfigPricingRepository : GenericRepository<DeliveryConfigPricing>, IDeliveryConfigPricingRepository
    {
        public DeliveryConfigPricingRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

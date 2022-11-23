using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PromotionProductRepository : GenericRepository<PromotionProduct>, IPromotionProductRepository
    {
        public PromotionProductRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

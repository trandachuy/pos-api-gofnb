using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PromotionProductCategoryRepository : GenericRepository<PromotionProductCategory>, IPromotionProductCategoryRepository
    {
        public PromotionProductCategoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

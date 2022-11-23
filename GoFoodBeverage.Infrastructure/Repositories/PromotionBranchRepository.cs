using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PromotionBranchRepository : GenericRepository<PromotionBranch>, IPromotionBranchRepository
    {
        public PromotionBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

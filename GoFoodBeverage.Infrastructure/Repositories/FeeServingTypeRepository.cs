using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FeeServingTypeRepository : GenericRepository<FeeServingType>, IFeeServingTypeRepository
    {
        public FeeServingTypeRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

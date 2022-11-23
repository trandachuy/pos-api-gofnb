using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PointHistoryRepository : GenericRepository<PointHistory>, IPointHistoryRepository
    {
        public PointHistoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}
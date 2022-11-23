using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderHistoryRepository : GenericRepository<OrderHistory>, IOrderHistoryRepository
    {
        public OrderHistoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

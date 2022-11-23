using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderFeeRepository : GenericRepository<OrderFee>, IOrderFeeRepository
    {
        public OrderFeeRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderItemRestoreRepository : GenericRepository<OrderItemRestore>, IOrderItemRestoreRepository
    {
        public OrderItemRestoreRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }


    }
}

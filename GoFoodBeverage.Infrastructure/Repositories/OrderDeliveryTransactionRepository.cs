using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderDeliveryTransactionRepository : GenericRepository<OrderDeliveryTransaction>, IOrderDeliveryTransactionRepository
    {
        public OrderDeliveryTransactionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

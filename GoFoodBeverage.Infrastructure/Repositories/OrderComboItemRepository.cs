using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderComboItemRepository : GenericRepository<OrderComboItem>, IOrderComboItemRepository
    {
        public OrderComboItemRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

    }
}

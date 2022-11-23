using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderRestoreRepository : GenericRepository<OrderRestore>, IOrderRestoreRepository
    {
        public OrderRestoreRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

    }
}

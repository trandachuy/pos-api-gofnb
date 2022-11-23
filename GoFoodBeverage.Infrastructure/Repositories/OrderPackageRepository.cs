using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderPackageRepository : GenericRepository<OrderPackage>, IOrderPackageRepository
    {
        public OrderPackageRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class MaterialInventoryHistoryRepository : GenericRepository<MaterialInventoryHistory>, IMaterialInventoryHistoryRepository
    {
        public MaterialInventoryHistoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

    }
}
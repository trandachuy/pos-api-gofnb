using System;
using System.Linq;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Unit> GetAllUnitsInStore(Guid? storeId)
        {
            var units = dbSet.Where(u => u.StoreId == storeId);

            return units;
        }
    }
}

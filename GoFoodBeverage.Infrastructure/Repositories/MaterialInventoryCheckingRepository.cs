using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class MaterialInventoryCheckingRepository : GenericRepository<MaterialInventoryChecking>, IMaterialInventoryCheckingRepository
    {
        public MaterialInventoryCheckingRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<MaterialInventoryChecking> GetMaterialInventoryCheckingByShiftId(Guid storeId, Guid shiftId)
        {
            var materialInventoryChecking = dbSet.Where(m => m.StoreId == storeId && m.ShiftId == shiftId);

            return materialInventoryChecking;
        }
    }
}

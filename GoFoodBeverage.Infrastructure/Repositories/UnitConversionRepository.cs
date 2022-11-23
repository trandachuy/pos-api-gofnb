using System;
using System.Linq;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class UnitConversionRepository : GenericRepository<UnitConversion>, IUnitConversionRepository
    {
        public UnitConversionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<UnitConversion> GetAllUnitConversionsInStore(Guid? storeId)
        {
            var units = dbSet.Where(u => u.StoreId == storeId);

            return units;
        }

        public IQueryable<UnitConversion> GetUnitConversionsByMaterialIdInStore(Guid materialId, Guid? storeId)
        {
            var units = dbSet
                .Where(u => u.MaterialId == materialId && u.StoreId == storeId);

            return units;
        }
    }
}

using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class BarcodeConfigRepository : GenericRepository<BarcodeConfig>, IBarcodeConfigRepository
    {
        public BarcodeConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<BarcodeConfig> GetBarcodeConfigByStoreIdAsync(Guid? storeId)
        {
            var barcodeConfig = await dbSet.FirstOrDefaultAsync(st => st.StoreId == storeId);

            return barcodeConfig;
        }
    }
}

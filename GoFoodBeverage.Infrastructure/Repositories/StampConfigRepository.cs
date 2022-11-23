using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StampConfigRepository : GenericRepository<StampConfig>, IStampConfigRepository
    {
        public StampConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<StampConfig> GetStampConfigByStoreIdAsync(Guid? storeId)
        {
            var stampConfig = await dbSet.FirstOrDefaultAsync(st => st.StoreId == storeId);

            return stampConfig;
        }

        public async Task<StampConfig> CreateDefaultStamConfigAsync(Guid? storeId)
        {
            var config = new StampConfig()
            {
                StoreId = storeId,
                StampType = EnumStampType.mm50x30,
                IsShowLogo = false,
                IsShowTime = true,
                IsShowNumberOfItem = true,
                IsShowNote = false,
            };

            dbSet.Add(config);
            await _dbContext.SaveChangesAsync();

            return config;
        }
    }
}

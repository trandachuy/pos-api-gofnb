using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreConfigRepository : GenericRepository<StoreConfig>, IStoreConfigRepository
    {
        public StoreConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<StoreConfig> QueryStoreConfigByStoreIdAsync(Guid storeId)
        {
            var storeConfig = _dbContext.StoreConfigs.Where(s => s.StoreId == storeId);

            return storeConfig;
        }

        public async Task<StoreConfig> GetStoreConfigByStoreIdAsync(Guid storeId)
        {
            var storeConfig = await _dbContext.StoreConfigs.FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (storeConfig != null) return storeConfig;

            var newStoreConfig = await CreateStoreConfigAsync(storeId);
            return newStoreConfig;
        }

        public async Task<StoreConfig> CreateStoreConfigAsync(Guid storeId)
        {
            var newStoreConfig = new StoreConfig()
            {
                StoreId = storeId
            };

            _dbContext.StoreConfigs.Add(newStoreConfig);
            await _dbContext.SaveChangesAsync();

            return newStoreConfig;
        }

        public async Task UpdateStoreConfigAsync(StoreConfig storeConfig)
        {
            _dbContext.StoreConfigs.Update(storeConfig);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStoreConfigAsync(StoreConfig storeConfig, string storeConfigIncreaseType)
        {
            switch (storeConfigIncreaseType)
            {
                case StoreConfigConstants.PURCHASE_ORDER_CODE:
                    storeConfig.CurrentMaxPurchaseOrderCode += 1;
                    break;

                case StoreConfigConstants.ORDER_CODE:
                    storeConfig.CurrentMaxOrderCode += 1;
                    break;

                case StoreConfigConstants.PRODUCT_CATEGORY_CODE:
                    storeConfig.CurrentMaxProductCategoryCode += 1;
                    break;

                case StoreConfigConstants.OPTION_CODE:
                    storeConfig.CurrentMaxOptionCode += 1;
                    break;

                case StoreConfigConstants.TOPPING_CODE:
                    storeConfig.CurrentMaxToppingCode += 1;
                    break;

                case StoreConfigConstants.MATERIAL_CODE:
                    storeConfig.CurrentMaxMaterialCode += 1;
                    break;
            }

            _dbContext.StoreConfigs.Update(storeConfig);
            await _dbContext.SaveChangesAsync();
        }
    }
}
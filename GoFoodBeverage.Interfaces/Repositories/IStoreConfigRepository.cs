using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStoreConfigRepository : IGenericRepository<StoreConfig>
    {
        IQueryable<StoreConfig> QueryStoreConfigByStoreIdAsync(Guid storeId);

        /// <summary>
        /// Get store configure. If the store configure has not been existed, create store configure with default values.
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>StoreConfig</returns>
        Task<StoreConfig> GetStoreConfigByStoreIdAsync(Guid storeId);

        Task<StoreConfig> CreateStoreConfigAsync(Guid storeId);

        Task UpdateStoreConfigAsync(StoreConfig storeConfig);

        Task UpdateStoreConfigAsync(StoreConfig storeConfig, string storeConfigIncreaseType);
    }
}

using GoFoodBeverage.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStampConfigRepository : IGenericRepository<StampConfig>
    {
        Task<StampConfig> GetStampConfigByStoreIdAsync(Guid? storeId);

        Task<StampConfig> CreateDefaultStamConfigAsync(Guid? storeId);
    }
}

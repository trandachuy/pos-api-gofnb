using GoFoodBeverage.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IBarcodeConfigRepository : IGenericRepository<BarcodeConfig>
    {
        Task<BarcodeConfig> GetBarcodeConfigByStoreIdAsync(Guid? storeId);
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class DeliveryConfigRepository : GenericRepository<DeliveryConfig>, IDeliveryConfigRepository
    {
        public DeliveryConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<DeliveryConfig> GetDeliveryConfigByDeliveryMethodIdAsync(Guid deliveryMethodId, Guid storeId)
        {
            var deliveryConfig = await dbSet
                .Where(dc => dc.DeliveryMethodId == deliveryMethodId && dc.StoreId == storeId)
                .Include(dcp => dcp.DeliveryConfigPricings)
                .FirstOrDefaultAsync();

            return deliveryConfig;
        }

        public async Task<DeliveryConfig> GetAhaMoveConfigByDeliveryMethodIdAsync(Guid deliveryMethodId, Guid storeId)
        {
            var deliveryConfig = await dbSet
                .Where(dc => dc.DeliveryMethodId == deliveryMethodId && dc.StoreId == storeId)
                .FirstOrDefaultAsync();

            return deliveryConfig;
        }
    }
}

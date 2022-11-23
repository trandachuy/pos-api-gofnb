using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class LoyaltyPointConfigRepository : GenericRepository<LoyaltyPointConfig>, ILoyaltyPointConfigRepository
    {
        public LoyaltyPointConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<LoyaltyPointConfig> GetLoyaltyPointConfigByStoreIdAsync(Guid storeId)
        {
            var data = dbSet.Where(m => m.StoreId == storeId);
            return data;
        }

    }
}

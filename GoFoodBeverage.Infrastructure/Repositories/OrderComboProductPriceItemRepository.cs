using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderComboProductPriceItemRepository : GenericRepository<OrderComboProductPriceItem>, IOrderComboProductPriceItemRepository
    {
        public OrderComboProductPriceItemRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// This method is used to get an object by the id
        /// </summary>
        /// <param name="id">For example: af1d1fc7-aab4-41a0-a016-0c9ef115e018</param>
        /// <returns>OrderComboProductPriceItem</returns>
        public async Task<OrderComboProductPriceItem> GetByIdAsync(Guid id, Guid? storeId) => await _dbContext.
            OrderComboProductPriceItems.
            SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == id);
    }
}

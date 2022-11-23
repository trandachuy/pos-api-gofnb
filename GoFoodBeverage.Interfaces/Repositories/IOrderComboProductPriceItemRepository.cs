using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOrderComboProductPriceItemRepository : IGenericRepository<OrderComboProductPriceItem>
    {

        /// <summary>
        /// This method is used to get an object by the id
        /// </summary>
        /// <param name="id">For example: af1d1fc7-aab4-41a0-a016-0c9ef115e018</param>
        /// <returns>OrderComboProductPriceItem</returns>
        Task<OrderComboProductPriceItem> GetByIdAsync(Guid id, Guid? storeId);
    }
}

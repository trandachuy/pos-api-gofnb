using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        IQueryable<Promotion> GetAllPromotionInStore(Guid storeId);

        /// <summary>
        /// Get Promotion by id and store Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<Promotion> GetPromotionByIdInStoreAsync(Guid id, Guid storeId);

        Task<Promotion> GetPromotionByNameInStoreAsync(string promotionName, Guid storeId);
    }
}

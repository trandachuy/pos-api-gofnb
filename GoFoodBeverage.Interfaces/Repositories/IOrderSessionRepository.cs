using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOrderSessionRepository : IGenericRepository<OrderSession>
    {
        /// <summary>
        /// Get kitchen order session 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        IQueryable<OrderSession> GetKitchenOrderSessionInStore(Guid? storeId, Guid? branchId);

        Task<OrderSession> GetOrderSessionByIdAsync(Guid orderSessionId, Guid? storeId);
    }
}

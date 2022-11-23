using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICustomerPointRepository : IGenericRepository<CustomerPoint>
    {
        IQueryable<CustomerPoint> GetAllCustomerPointInStore(Guid? storeId);

        IQueryable<CustomerPoint> GetCustomerPointsbyAccumulatedPoint(int accumulatedPoint);

        /// <summary>
        /// This method is used to count the total number of users at any membership level.
        /// For example: The accumulated point of classic membership is 0, 
        /// the next accumulated point is 1000 which is a gold member.
        /// </summary>
        /// <param name="storeId">The store id, for example: 8d6c95cc-b181-4404-9b1f-e1d12346eec0</param>
        /// <param name="minAccumulatedPoint">The current accumulated point of membership level.</param>
        /// <param name="maxAccumulatedPoint">The next accumulated point of the membership level.</param>
        /// <returns>int</returns>
        Task<int> CountCustomerPointByStoreIdAsync(
            Guid? storeId,
            int minAccumulatedPoint,
            int maxAccumulatedPoint
        );
    }
}

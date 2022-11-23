using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CustomerPointRepository : GenericRepository<CustomerPoint>, ICustomerPointRepository
    {
        public CustomerPointRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<CustomerPoint> GetCustomerPointsbyAccumulatedPoint(int accumulatedPoint)
        {
            var customerPoints = dbSet.Where(c => c.AccumulatedPoint >= accumulatedPoint);
            return customerPoints;
        }

        /// <summary>
        /// This method is used to count the total number of users at any membership level.
        /// </summary>
        /// <param name="storeId">The store id, for example: 8d6c95cc-b181-4404-9b1f-e1d12346eec0</param>
        /// <param name="minAccumulatedPoint">The current accumulated point of membership level.</param>
        /// <param name="maxAccumulatedPoint">The next accumulated point of the membership level.</param>
        /// <returns>int</returns>
        public async Task<int> CountCustomerPointByStoreIdAsync(
            Guid? storeId,
            int minAccumulatedPoint,
            int maxAccumulatedPoint
        )
        {
            int result = await dbSet.
                CountAsync(cp =>
                    cp.Customer.StoreId == storeId &&
                    cp.AccumulatedPoint >= minAccumulatedPoint &&
                    (maxAccumulatedPoint == 0 ? true : cp.AccumulatedPoint < maxAccumulatedPoint)
                );
            return result;
        }

        public IQueryable<CustomerPoint> GetAllCustomerPointInStore(Guid? storeId)
        {
            var customerPoints = dbSet.Where(c => c.StoreId == storeId);
            return customerPoints;
        }
    }
}

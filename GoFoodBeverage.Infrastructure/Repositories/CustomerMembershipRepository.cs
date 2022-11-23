using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CustomerMembershipRepository : GenericRepository<CustomerMembershipLevel>, ICustomerMembershipRepository
    {
        public CustomerMembershipRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public bool CheckCustomerMemberShipByNameInStore(string name, Guid? storeId)
        {
            var customerMembership = dbSet.FirstOrDefault(c => c.StoreId == storeId && c.Name.ToLower() == name.ToLower());
            return customerMembership == null ? false : true;
        }

        public IQueryable<CustomerMembershipLevel> GetAllCustomerMembershipInStore(Guid? storeId)
        {
            var customerMemberships = dbSet.Where(c => c.StoreId == storeId);
            return customerMemberships;
        }

        public Task<CustomerMembershipLevel> GetCustomerMembershipDetailByIdAsync(Guid customerMembershipId, Guid? storeId)
        {
            var customerMembership = dbSet.Where(c => c.StoreId == storeId && c.Id == customerMembershipId).FirstOrDefaultAsync();

            return customerMembership;
        }

        public bool CheckCustomerMemberShipByAccumulatedPointInStore(int accumulatedPoint, Guid? storeId)
        {
            var customerMembership = dbSet.FirstOrDefault(c => c.StoreId == storeId && c.AccumulatedPoint == accumulatedPoint);
            return customerMembership == null ? false : true;
        }
    }
}

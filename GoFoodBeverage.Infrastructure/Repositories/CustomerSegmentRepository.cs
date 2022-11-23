using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CustomerSegmentRepository : GenericRepository<CustomerSegment>, ICustomerSegmentRepository
    {
        public CustomerSegmentRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<CustomerSegment> GetAllCustomerSegmentsInStore(Guid storeId)
        {
            var customerSegments = dbSet
                .Where(s => s.StoreId == storeId)
                .Include(s => s.CustomerCustomerSegments)
                .ThenInclude(cs => cs.Customer);

            return customerSegments;
        }

        public Task<CustomerSegment> GetCustomerSegmentByNameInStoreAsync(string customerSegmentName, Guid storeId)
        {
            var customerSegment = dbSet.FirstOrDefaultAsync(s => s.Name.ToLower().Equals(customerSegmentName.ToLower()) && s.StoreId == storeId);

            return customerSegment;
        }

        public Task<CustomerSegment> GetCustomerSegmentDetailByIdAsync(Guid customerSegmentId, Guid? storeId)
        {
            var customerSegment = dbSet
                .Where(c => c.StoreId == storeId && c.Id == customerSegmentId)
                .Include(c => c.CustomerSegmentConditions)
                .FirstOrDefaultAsync();

            return customerSegment;
        }

        public Task<CustomerSegment> CheckExistCustomerSegmentNameInStoreAsync(Guid customerSegmentId, string customerSegmentName, Guid storeId)
        {
            var customerSegment = dbSet.FirstOrDefaultAsync(c => c.Id != customerSegmentId && c.Name.Trim().ToLower().Equals(customerSegmentName.Trim().ToLower()) && c.StoreId == storeId);

            return customerSegment;
        }
    }
}

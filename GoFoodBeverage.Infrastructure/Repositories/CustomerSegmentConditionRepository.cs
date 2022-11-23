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
    public class CustomerSegmentConditionRepository : GenericRepository<CustomerSegmentCondition>, ICustomerSegmentConditionRepository
    {
        public CustomerSegmentConditionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<CustomerSegmentCondition> GetAllConditionsByCustomerSegmentId(Guid customerSegmentId)
        {
            var conditions = dbSet
                .Where(c => c.CustomerSegmentId == customerSegmentId);

            return conditions;
        }
    }
}

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
    public class CustomerCustomerSegmentRepository : GenericRepository<CustomerCustomerSegment>, ICustomerCustomerSegmentRepository
    {
        public CustomerCustomerSegmentRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}

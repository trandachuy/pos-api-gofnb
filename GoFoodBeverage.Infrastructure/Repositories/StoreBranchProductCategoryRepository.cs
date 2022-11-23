using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreBranchProductCategoryRepository : GenericRepository<StoreBranchProductCategory>, IStoreBranchProductCategoryRepository
    {
        public StoreBranchProductCategoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

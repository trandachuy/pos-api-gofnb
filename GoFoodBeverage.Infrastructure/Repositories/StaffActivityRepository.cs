using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StaffActivityRepository : GenericRepository<StaffActivity>, IStaffActivityRepository
    {
        public StaffActivityRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}
 
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PackageDurationByMonthRepository : GenericRepository<PackageDurationByMonth>, IPackageDurationByMonthRepository
    {
        public PackageDurationByMonthRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

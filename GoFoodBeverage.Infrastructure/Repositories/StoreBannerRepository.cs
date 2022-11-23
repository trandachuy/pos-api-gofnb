using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreBannerRepository : GenericRepository<StoreBanner>, IStoreBannerRepository
    {
        public StoreBannerRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}
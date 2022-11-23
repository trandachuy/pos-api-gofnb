using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PlatformRepository : GenericRepository<Platform>, IPlatformRepository
    {
        public PlatformRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Platform> GetActivePlatforms()
        {
            var platforms = dbSet.Where(p => p.StatusId == (int)EnumStatus.Active);

            return platforms;
        }
    }
}

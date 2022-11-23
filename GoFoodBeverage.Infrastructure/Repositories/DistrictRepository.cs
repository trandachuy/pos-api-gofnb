using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        public DistrictRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<District> GetDistrictsByCityId(Guid cityId)
        {
            IQueryable<District> districts = dbSet.Where(c => c.CityId == cityId);
            return districts;
        }
    }
}

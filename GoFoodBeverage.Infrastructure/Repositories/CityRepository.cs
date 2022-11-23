using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<City> GetCitiesByCountryId(Guid countryId)
        {
            IQueryable<City> cities = dbSet.Where(c => c.CountryId == countryId);
            return cities;
        }
    }
}

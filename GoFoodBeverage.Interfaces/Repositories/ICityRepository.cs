using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICityRepository : IGenericRepository<City>
    {
        IQueryable<City> GetCitiesByCountryId(Guid countryId);
    }
}

using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public Task<Country> GetCountryByIso2Async(string iso2)
        {
            var country = dbSet.FirstOrDefaultAsync(c => c.Iso == iso2);
            return country;
        }

        public Task<Country> GetCountryByIdAsync(Guid countryId)
        {
            var country = dbSet.FirstOrDefaultAsync(c => c.Id == countryId);
            return country;
        }

        public async Task<Country> GetCountryByStoreIdAsync(Guid? storeId)
        {
            Country country = await dbSet
                .FirstOrDefaultAsync(country =>
                    country.Id == (_dbContext
                      .Stores
                      .Where(s =>
                      s.Id == storeId
                      )
                      .Select(s => s.Address.CountryId)
                      .FirstOrDefault()
                  )
                );
            return country;
        }
    }
}

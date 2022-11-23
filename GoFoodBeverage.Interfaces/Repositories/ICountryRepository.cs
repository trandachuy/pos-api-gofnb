using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<Country> GetCountryByIso2Async(string iso2);

        Task<Country> GetCountryByIdAsync(Guid countryId);

        Task<Country> GetCountryByStoreIdAsync(Guid? storeId);
    }
}

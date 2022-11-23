using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        /// <summary>
        /// Find address information by address id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Address> GetAddressByIdAsync(Guid id);
    }
}

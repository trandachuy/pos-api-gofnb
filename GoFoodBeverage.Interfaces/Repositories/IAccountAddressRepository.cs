using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IAccountAddressRepository : IGenericRepository<AccountAddress>
    {
        IQueryable<AccountAddress> GetAccountAddressesByAccountIdAsync(Guid accountId);
    }
}

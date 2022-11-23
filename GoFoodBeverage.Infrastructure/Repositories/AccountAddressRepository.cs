using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AccountAddressRepository : GenericRepository<AccountAddress>, IAccountAddressRepository
    {
        public AccountAddressRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<AccountAddress> GetAccountAddressesByAccountIdAsync(Guid accountId)
        {
            var accountAddresses = dbSet.Where(a => a.AccountId == accountId).Include(a => a.Account);

            return accountAddresses;
        }
    }
}
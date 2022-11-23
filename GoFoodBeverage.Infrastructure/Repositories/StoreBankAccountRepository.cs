using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreBankAccountRepository : GenericRepository<StoreBankAccount>, IStoreBankAccountRepository
    {
        public StoreBankAccountRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public Task<StoreBankAccount> GetStoreBankAccountByStoreIdAsync(Guid? storeId)
        {
            var storeBankAccount = dbSet.FirstOrDefaultAsync(s => s.StoreId == storeId);

            return storeBankAccount;
        }
    }
}

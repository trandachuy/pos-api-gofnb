using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStoreBankAccountRepository : IGenericRepository<StoreBankAccount>
    {
        Task<StoreBankAccount> GetStoreBankAccountByStoreIdAsync(Guid? storeId);
    }
}

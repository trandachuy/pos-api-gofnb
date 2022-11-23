using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class TaxRepository : GenericRepository<Tax>, ITaxRepository
    {
        public TaxRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Tax> GetAllTaxInStore(Guid storeId)
        {
            var taxes = dbSet.Where(s => s.StoreId == storeId);

            return taxes;
        }

        /// <summary>
        /// Get tax by Id, store Id and branchId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public IQueryable<Tax> GetTaxById(Guid id, Guid? storeId)
        {
            var tax = dbSet.Where(m => m.StoreId == storeId && m.Id == id);
            return tax;
        }
    }
}

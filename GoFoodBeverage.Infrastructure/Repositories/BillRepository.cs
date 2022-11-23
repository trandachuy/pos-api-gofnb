using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class BillRepository : GenericRepository<BillConfiguration>, IBillRepository
    {
        public BillRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        /// <summary>
        /// Get bill by Bill size, store Id
        /// </summary>
        /// <param name="size"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public IQueryable<BillConfiguration> GetBillConfigurationByFrameSize(EnumBillFrameSize size, Guid? storeId)
        {
            var bill = dbSet.Where(m => m.StoreId == storeId && m.BillFrameSize == size);
            return bill;
        }

        /// <summary>
        /// Get default bill by Bill size, store Id
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public IQueryable<BillConfiguration> GetDefaultBillConfigurationByStore(Guid? storeId)
        {
            var bill = dbSet.Where(m => m.StoreId == storeId);
            return bill;
        }
    }
}

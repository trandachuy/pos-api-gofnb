using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using System.Collections.Generic;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FeeRepository : GenericRepository<Fee>, IFeeRepository
    {
        public FeeRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Fee> GetAllFeesInStore(Guid? storeId)
        {
            var fees = dbSet.Where(o => o.StoreId == storeId);

            return fees;
        }

        public async Task<Fee> GetFeeByIdInStoreAsync(Guid id, Guid storeId)
        {
            var fee = await dbSet.Where(m => m.Id == id && m.StoreId == storeId)
                .Include(p => p.FeeServingTypes)
                .Include(p => p.FeeBranches).ThenInclude(pb => pb.Branch)
                .FirstOrDefaultAsync();

            return fee;
        }

        public IQueryable<Fee> GetAllFeesByListIdInStore(IQueryable<Guid> listFeeIds, Guid? storeId)
        {
            var fees = dbSet.Where(m => m.StoreId == storeId && listFeeIds.Contains(m.Id));

            return fees;
        }

        public IQueryable<Fee> GetFeesForCreateOrder(Guid? storeId, IEnumerable<Guid> orderFeeIds)
        {
            var fees = dbSet.Where(o => o.StoreId == storeId && orderFeeIds.Contains(o.Id))
                .Select(x => new Fee
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    IsPercentage = x.IsPercentage
                });

            return fees;
        }
    }
}

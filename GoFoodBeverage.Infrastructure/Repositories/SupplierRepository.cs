using System;
using System.Linq;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Supplier> GetAllSuppliersInStore(Guid? storeId)
        {
            var suppliers = dbSet.Where(s => s.StoreId == storeId);

            return suppliers;
        }

        public async Task<Supplier> GetSupplierByIdInStoreAsync(Guid id, Guid storeId)
        {
            var supplier = await dbSet.Where(s => s.Id == id && s.StoreId == storeId)
                .Include(s => s.Address).ThenInclude(a => a.Country)
                .Include(s => s.Address).ThenInclude(a => a.City)
                .Include(s => s.Address).ThenInclude(a => a.District)
                .Include(s => s.Address).ThenInclude(a => a.Ward)
                .Include(s => s.Address).ThenInclude(a => a.State)
                .FirstOrDefaultAsync();

            return supplier;
        }
    }
}

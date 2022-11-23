using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Material> GetAllMaterialsActivatedInStore(Guid? storeId)
        {
            var materials = dbSet.Where(m => m.StoreId == storeId && m.IsActive.HasValue && m.IsActive.Value == true);

            return materials;
        }

        public IQueryable<Material> GetAllMaterialsInStore(Guid? storeId)
        {
            var materials = dbSet.Where(m => m.StoreId == storeId);

            return materials;
        }

        public IQueryable<Material> GetAllMaterialsInStoreByIds(Guid? storeId, IEnumerable<Guid> materialIds)
        {
            var materials = dbSet.Where(m => m.StoreId == storeId && materialIds.Any(mid => mid == m.Id));

            return materials;
        }

        public IQueryable<Material> GetAllMaterialsByCategoryId(Guid? storeId, Guid materialCategoryId)
        {
            var materials = dbSet.Where(m => m.StoreId == storeId && m.MaterialCategoryId == materialCategoryId);

            return materials;
        }

        public async Task<Material> GetMaterialByIdInStoreAsync(Guid? id, Guid? storeId)
        {
            var material = await dbSet
                .Where(m => m.Id == id && m.StoreId == storeId)
                .Include(m => m.MaterialCategory)
                .Include(m => m.Unit)
                .Include(m => m.MaterialInventoryBranches.OrderBy(m => m.Position)).ThenInclude(mib => mib.Branch)
                .FirstOrDefaultAsync();

            return material;
        }

    }
}

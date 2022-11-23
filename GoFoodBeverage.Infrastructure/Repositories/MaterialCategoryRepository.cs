using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class MaterialCategoryRepository : GenericRepository<MaterialCategory>, IMaterialCategoryRepository
    {
        public MaterialCategoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<MaterialCategory> GetAllMaterialCategoriesInStore(Guid storeId)
        {
            var materialCategories = dbSet.Where(s => s.StoreId == storeId);

            return materialCategories;
        }

        public async Task<MaterialCategory> GetMaterialCategoryByIdInStoreAsync(Guid Id, Guid storeId)
        {
            var materialCategory = await dbSet
                .Where(m => m.Id == Id && m.StoreId == storeId)
                .Include(p => p.Materials)
                .FirstOrDefaultAsync();

            return materialCategory;
        }
    }
}

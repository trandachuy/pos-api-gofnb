using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<ProductCategory> GetAllProductCategoriesInStore(Guid storeId)
        {
            var productCategories = dbSet.Where(s => s.StoreId == storeId);

            return productCategories;
        }

        public Task<ProductCategory> GetProductCategoryByIdAsync(Guid productCategoryId)
        {
            var productCategory = dbSet.FirstOrDefaultAsync(p => p.Id == productCategoryId);

            return productCategory;
        }

        public Task<ProductCategory> GetProductCategoryByNameInStoreAsync(string productCategoryName, Guid storeId)
        {
            var productCategory = dbSet.FirstOrDefaultAsync(p => p.Name.Trim().ToLower().Equals(productCategoryName.Trim().ToLower()) && p.StoreId == storeId);

            return productCategory;
        }

        public Task<bool> CheckExistProductCategoryAsync(Guid? productCategoryId, Guid? storeId)
        {
            var productCategory = dbSet.FirstOrDefaultAsync(s => s.StoreId == storeId && s.Id == productCategoryId);
            var result = (productCategory?.Result) != null;

            return Task.FromResult(result);
        }

        public Task<ProductCategory> CheckExistProductCategoryNameInStoreAsync(Guid productCategoryId, string productCategoryName, Guid storeId)
        {
            var productCategory = dbSet.FirstOrDefaultAsync(p => p.Id != productCategoryId && p.Name.Trim().ToLower().Equals(productCategoryName.Trim().ToLower()) && p.StoreId == storeId);

            return productCategory;
        }

        public Task<ProductCategory> GetProductCategoryDetailByIdAsync(Guid productCategoryId, Guid? storeId)
        {
            var productCategory = dbSet
               .Where(p => p.StoreId == storeId && p.Id == productCategoryId)
               .Include(p => p.ProductProductCategories)
               .ThenInclude(pc => pc.Product)
               .Include(p => p.StoreBranchProductCategories)
               .ThenInclude(s => s.StoreBranch)
               .FirstOrDefaultAsync();

            return productCategory;
        }
    }
}

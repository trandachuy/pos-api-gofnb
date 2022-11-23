using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductProductCategoryRepository : GenericRepository<ProductProductCategory>, IProductProductCategoryRepository
    {
        public ProductProductCategoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<ProductProductCategory> GetAllIncludedProductUnitByProductCategoryId(Guid productCategoryId, Guid? storeId)
        {
            var productProductCategory = _dbContext.ProductProductCategories.Where(c => c.StoreId == storeId && c.ProductCategoryId == productCategoryId && c.Product.IsActive == true)
                .Include(x => x.Product).ThenInclude(x => x.Unit);

            return productProductCategory;
        }

        public IQueryable<ProductProductCategory> GetAllByProductCategoryId(Guid productCategoryId)
        {
            var productProductCategory = _dbContext.ProductProductCategories.Where(c => c.ProductCategoryId == productCategoryId);

            return productProductCategory;
        }

        public IQueryable<ProductProductCategory> GetAllProductInStoreActive(Guid storeId)
        {
            var productProductCategory = _dbContext.ProductProductCategories
                .Where(c => c.Product.StoreId == storeId && c.Product.StatusId == (int)EnumStatus.Active && c.Product.IsActive == true && !c.Product.IsTopping)
                .Include(p => p.Product)
                .Include(x => x.Product.ProductPrices)
                .Include(p => p.Product.Unit);

            return productProductCategory;
        }
    }
}

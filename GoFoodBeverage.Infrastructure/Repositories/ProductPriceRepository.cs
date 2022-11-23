using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductPriceRepository : GenericRepository<ProductPrice>, IProductPriceRepository
    {
        public ProductPriceRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<ProductPrice> GetAllProductPriceInStore(Guid? storeId)
        {
            var query = dbSet
                .Where(p => p.Product.StoreId == storeId)
                .Include(p => p.Product).ThenInclude(x=>x.Tax);

            return query;
        }

        public IQueryable<ProductPrice> GetAllProductPriceWithProductId(Guid? storeId, Guid? productId)
        {
            var query = dbSet
                .Where(p => p.Product.StoreId == storeId && p.Product.Id == productId)
                .Include(p => p.Product);

            return query;
        }
    }
}

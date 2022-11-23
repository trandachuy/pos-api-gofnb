using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Linq;
using System;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductToppingRepository : GenericRepository<ProductTopping>, IProductToppingRepository
    {
        public ProductToppingRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<ProductTopping> GetToppingsByProductId(Guid productId, Guid? storeId)
        {
            var toppings = dbSet.Where(t => t.StoreId == storeId && t.ProductId == productId);
            return toppings;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ProductPriceMaterialRepository : GenericRepository<ProductPriceMaterial>, IProductPriceMaterialRepository
    {
        public ProductPriceMaterialRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<ProductPriceMaterial> GetProductPriceMaterialsByMaterialIds(IEnumerable<Guid> materialIds)
        {
            var productPriceMaterials = dbSet.Where(ppm => materialIds.Any(mid => mid == ppm.MaterialId));

            return productPriceMaterials;
        }
    }
}

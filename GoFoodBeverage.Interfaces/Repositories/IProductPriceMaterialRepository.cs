using System;
using System.Linq;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductPriceMaterialRepository : IGenericRepository<ProductPriceMaterial>
    {
        IQueryable<ProductPriceMaterial> GetProductPriceMaterialsByMaterialIds(IEnumerable<Guid> materialIds);
    }
}

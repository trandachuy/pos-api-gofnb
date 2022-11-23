using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductProductCategoryRepository : IGenericRepository<ProductProductCategory>
    {
        IQueryable<ProductProductCategory> GetAllIncludedProductUnitByProductCategoryId(Guid productCategoryId, Guid? storeId);

        IQueryable<ProductProductCategory> GetAllByProductCategoryId(Guid productCategoryId);

        IQueryable<ProductProductCategory> GetAllProductInStoreActive(Guid storeId);
    }
}

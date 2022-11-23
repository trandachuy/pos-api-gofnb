
using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductPriceRepository : IGenericRepository<ProductPrice>
    {
        IQueryable<ProductPrice> GetAllProductPriceInStore(Guid? storeId);

        IQueryable<ProductPrice> GetAllProductPriceWithProductId(Guid? storeId, Guid? productId);
    }
}

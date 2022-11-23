using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductToppingRepository : IGenericRepository<ProductTopping>
    {
        IQueryable<ProductTopping> GetToppingsByProductId(Guid productId, Guid? storeId);
    }
}

using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
    {
        IQueryable<ProductCategory> GetAllProductCategoriesInStore(Guid storeId);

        Task<ProductCategory> GetProductCategoryByIdAsync(Guid productCategoryId);

        Task<ProductCategory> GetProductCategoryDetailByIdAsync(Guid productCategoryId, Guid? storeId);

        Task<ProductCategory> GetProductCategoryByNameInStoreAsync(string productCategoryName, Guid storeId);

        Task<bool> CheckExistProductCategoryAsync(Guid? productCategoryId, Guid? storeId);

        Task<ProductCategory> CheckExistProductCategoryNameInStoreAsync(Guid productCategoryId, string productCategoryName, Guid storeId);
    }
}

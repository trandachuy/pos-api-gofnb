using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IQueryable<Product> GetAllProductInStore(Guid storeId);

        IQueryable<Product> GetAllProductInStoreActive(Guid storeId);

        IQueryable<Product> GetProductByIdInStore(Guid storeId, Guid id);

        IQueryable<Product> GetAllToppingActivatedInStore(Guid storeId);

        Task<Product> GetProductByNameInStoreAsync(string productName, Guid storeId);

        Task<Product> GetPOSProductDetailByIdAsync(Guid id);

        Task<bool> CheckEditProductByNameInStoreAsync(Guid productId, string productName, Guid storeId);
        
        /// <summary>
        /// Update product using UpdateProductRequest model
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        Task<Tuple<bool, int?, string>> UpdateProductAsync(Guid storeId, UpdateProductModel updateModel);

        IQueryable<Product> GetAllProductIncludedProductUnit(Guid storeId);

        /// <summary>
        /// Get list toppings belong to product
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="toppingIds"></param>
        /// <returns></returns>
        IQueryable<Product> GetAllToppingBelongToProduct(Guid storeId, Guid productId);

        Task<Product> GetProductActiveByNameInStoreAsync(string productName, Guid storeId);
    }
}

using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IMaterialCategoryRepository : IGenericRepository<MaterialCategory>
    {
        IQueryable<MaterialCategory> GetAllMaterialCategoriesInStore(Guid storeId);

        /// <summary>
        /// Get group permission by id and store Id included Materials
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<MaterialCategory> GetMaterialCategoryByIdInStoreAsync(Guid id, Guid storeId);
    }
}

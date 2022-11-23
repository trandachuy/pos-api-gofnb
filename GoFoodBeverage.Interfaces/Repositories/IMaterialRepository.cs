using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IMaterialRepository : IGenericRepository<Material>
    {
        IQueryable<Material> GetAllMaterialsInStore(Guid? storeId);

        IQueryable<Material> GetAllMaterialsActivatedInStore(Guid? storeId);

        /// <summary>
        /// Get all material by list materialId in a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="materialIds"></param>
        /// <returns></returns>
        IQueryable<Material> GetAllMaterialsInStoreByIds(Guid? storeId, IEnumerable<Guid> materialIds);

        IQueryable<Material> GetAllMaterialsByCategoryId(Guid? storeId, Guid materialCategoryId);

        /// <summary>
        /// Get material by id and includes some references
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<Material> GetMaterialByIdInStoreAsync(Guid? id, Guid? storeId);
    }
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Language.Dto;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ILanguageStoreRepository : IGenericRepository<LanguageStore>
    {
        /// <summary>
        /// Get all languages of store
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<IList<LanguageStoreDto>> GetLanguageStoreByStoreId(Guid? storeId);

        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LanguageStore> GetLanguageStoreById(Guid? id);

        /// <summary>
        /// Get all languages of the store was published
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<List<LanguageStoreDto>> GetLanguageByStoreIdAndIsPublish(Guid? storeId);
    }
}

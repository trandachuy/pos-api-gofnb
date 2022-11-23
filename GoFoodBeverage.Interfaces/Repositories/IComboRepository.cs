using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IComboRepository : IGenericRepository<Combo>
    {
        IQueryable<Combo> GetAllCombosInStore(Guid storeId);

        IQueryable<Combo> GetAllCombosInStoreActivies(Guid storeId);

        IQueryable<Combo> GetAllCombosInStoreInclude(Guid storeId);

        /// <summary>
        /// Get combo by Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns>combo object</returns>
        Task<Combo> GetComboByIdAsync(Guid comboId, Guid? storeId);

        IQueryable<Combo> GetComboByIdWithoutTracking(Guid storeId, Guid comboId);
    }
}

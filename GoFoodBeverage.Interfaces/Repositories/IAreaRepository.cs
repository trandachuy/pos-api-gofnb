using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IAreaRepository : IGenericRepository<Area>
    {
        #region Methods applied on Admin 
        IQueryable<Area> GetAllAreasInStore(Guid? storeId);

        /// <summary>
        /// Get all active areas in specific branch
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<Area> GetAllAreasActiveByStoreId(Guid? storeId);

        /// <summary>
        /// Get all areas in specific branch
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<Area> GetAreasByStoreBranchId(Guid? storeId, Guid? branchId);

        /// <summary>
        /// Get all active areas in specific branch
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<Area> GetActiveAreasByStoreBranchId(Guid? storeId, Guid? branchId);

        /// <summary>
        /// Get area by Id, store Id and branchId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <param name="storeBranchId"></param>
        /// <returns></returns>
        IQueryable<Area> GetAreaById(Guid id, Guid? storeId, Guid? storeBranchId);
        #endregion

        #region Methods applied on POS
        /// <summary>
        /// Get all areas and tables of area are using
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<Area> POS_GetAreasInUseByStoreBranchId(Guid? storeId, Guid? storeBranchId);


        /// <summary>
        /// Get all areas and include table in use and available
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<Area> POS_GetActiveAreasByStoreBranchId(Guid? storeId, Guid? storeBranchId);

        #endregion
    }
}

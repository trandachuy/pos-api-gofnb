using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IAreaTableRepository : IGenericRepository<AreaTable>
    {
        /// <summary>
        /// Get all area tables in specific branch id
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="storeBranchId"></param>
        /// <returns></returns>
        IQueryable<AreaTable> GetAllAreaTablesByStoreBranchId(Guid? storeId, Guid? storeBranchId);

        IQueryable<AreaTable> GetAllAreaTablesByStoreId(Guid? storeId);

        Task<AreaTable> GetAreaTableByIdAsync(Guid areaTableId, Guid? storeId);

        Task<AreaTable> CheckExistTableNameInAreaAsync(Guid tableId, string tableName, Guid areaId);
    }
}

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AreaRepository : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        #region Admin
        public IQueryable<Area> GetAllAreasInStore(Guid? storeId)
        {
            var areas = dbSet.Where(m => m.StoreId == storeId);

            return areas;
        }

        public IQueryable<Area> GetAllAreasActiveByStoreId(Guid? storeId)
        {
            var areas = dbSet.Where(a => a.StoreId == storeId && a.IsActive == true);

            return areas;
        }

        /// <summary>
        /// Get all areas in specific branch
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IQueryable<Area> GetAreasByStoreBranchId(Guid? storeId, Guid? storeBranchId)
        {
            var areaManagements = dbSet.Where(m => m.StoreId == storeId && m.StoreBranchId == storeBranchId);
            return areaManagements;
        }

        /// <summary>
        /// Get all active areas in specific branch
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IQueryable<Area> GetActiveAreasByStoreBranchId(Guid? storeId, Guid? storeBranchId)
        {
            var areas = dbSet.Where(a => a.StoreId == storeId && a.StoreBranchId == storeBranchId && a.IsActive == true)
                .Include(p => p.AreaTables);
            return areas;
        }

        /// <summary>
        /// Get area by Id, store Id and branchId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <param name="storeBranchId"></param>
        /// <returns></returns>
        public IQueryable<Area> GetAreaById(Guid id, Guid? storeId, Guid? storeBranchId)
        {
            var area = dbSet.Where(m => m.StoreId == storeId && m.StoreBranchId == storeBranchId && m.Id == id);
            return area;
        }
        #endregion

        #region POS
        /// <summary>
        /// Get all areas and tables of area are using
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IQueryable<Area> POS_GetAreasInUseByStoreBranchId(Guid? storeId, Guid? storeBranchId)
        {
            var areas = dbSet
                .Where(a => a.StoreId == storeId && a.StoreBranchId == storeBranchId)
                .Include(p => p.AreaTables)
                .ThenInclude(m => m.Orders.Where(oder =>
                    ((oder.StatusId == EnumOrderStatus.New) ||
                    (oder.StatusId == EnumOrderStatus.Processing)) &&
                    (oder.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid))
                .OrderBy(o => o.CreatedTime));
            return areas;
        }

        /// <summary>
        /// Get all areas and include table in use and available
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IQueryable<Area> POS_GetActiveAreasByStoreBranchId(Guid? storeId, Guid? storeBranchId)
        {
            var areas = dbSet
                .Where(a => a.StoreId == storeId && a.StoreBranchId == storeBranchId && a.IsActive == true)
                .Include(p => p.AreaTables)
                .ThenInclude(m => m.Orders.Where(oder =>
                    (int)oder.StatusId == (int)EnumOrderStatus.New ||
                    (int)oder.StatusId == (int)EnumOrderStatus.Processing ||
                    oder == null)
                .OrderBy(o => o.CreatedTime));

            return areas;
        }
        #endregion

    }
}

using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AreaTableRepository : GenericRepository<AreaTable>, IAreaTableRepository
    {
        public AreaTableRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<AreaTable> GetAllAreaTablesByStoreId(Guid? storeId)
        {
            var tables = dbSet.Where(t => t.StoreId == storeId);

            return tables;
        }

        public IQueryable<AreaTable> GetAllAreaTablesByStoreBranchId(Guid? storeId, Guid? storeBranchId)
        {
            var tables = dbSet.Where(t => t.Area.StoreId == storeId && t.Area.StoreBranchId == storeBranchId);

            return tables;
        }

        public Task<AreaTable> GetAreaTableByIdAsync(Guid areaTableId, Guid? storeId)
        {
            var table = dbSet
                .Where(t => t.StoreId == storeId && t.Id == areaTableId)
                .Include(t => t.Area)
                .FirstOrDefaultAsync();

            return table;
        }

        public Task<AreaTable> CheckExistTableNameInAreaAsync(Guid tableId, string tableName, Guid areaId)
        {
            var table = dbSet.FirstOrDefaultAsync(t => t.Id != tableId && t.Name.ToLower() == tableName.ToLower() && t.AreaId == areaId);

            return table;
        }
    }
}

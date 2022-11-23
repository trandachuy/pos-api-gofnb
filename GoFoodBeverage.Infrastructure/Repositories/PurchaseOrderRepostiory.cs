using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static GoFoodBeverage.Common.Extensions.PagingExtensions;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PurchaseOrderRepostiory : GenericRepository<PurchaseOrder>, IPurchaseOrderRepostiory
    {
        public PurchaseOrderRepostiory(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        /// <summary>
        /// Get all purchase order by Store Id (pagingnation)
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="KeySearch"></param>
        /// <returns></returns>
        public async Task<Pager<PurchaseOrder>> GetAllPurchaseOrderByStoreId(Guid storeId, int pageNumber, int pageSize, string keySearch)
        {
            Pager<PurchaseOrder> purchaseOrders;
            if (string.IsNullOrEmpty(keySearch))
            {
                purchaseOrders = await dbSet
                    .Where(m => m.StoreId.Value == storeId)
                    .Include(p => p.StoreBranch)
                    .Include(p => p.Supplier)
                    .Include(p => p.PurchaseOrderMaterials)
                    .OrderByDescending(p => p.CreatedTime)
                    .ToPaginationAsync(pageNumber, pageSize);
                return purchaseOrders;
            }

            keySearch = keySearch.Trim().ToLower();
            purchaseOrders = await dbSet
                .Where(m => m.StoreId.Value == storeId && m.Supplier.Name.Contains(keySearch))
                .Include(p => p.StoreBranch)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderMaterials)
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(pageNumber, pageSize);

            return purchaseOrders;
        }

        /// <summary>
        /// Get purchase order by Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<PurchaseOrder> GetPurchaseOrderByIdInStoreAsync(Guid Id, Guid storeId)
        {
            var purchaseOrder = await dbSet.Where(m => m.Id == Id && m.StoreId == storeId)
                .Include(p => p.StoreBranch)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(p => p.Material).ThenInclude(m => m.Unit)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(m => m.Unit)
                .FirstOrDefaultAsync();
            return purchaseOrder;
        }

        public async Task<IList<PurchaseOrder>> GetAllPurchaseOrderByMaterialIdInStoreAsync(Guid materialId, Guid storeId)
        {
            var purchaseOrder = await dbSet
                .Where(m => m.StoreId == storeId)
                .Include(p => p.PurchaseOrderMaterials.Where(po => po.MaterialId == materialId)).ThenInclude(p => p.Material)
                .ToListAsync();

            return purchaseOrder;
        }

        public async Task<IList<PurchaseOrder>> GetAllPurchaseOrderBySupplierIdInStoreAsync(Guid supplierId, Guid storeId)
        {
            var purchaseOrder = await dbSet
                .Where(p => p.StoreId == storeId && p.SupplierId == supplierId)
                .Include(p => p.Supplier)
                .ToListAsync();

            return purchaseOrder;
        }

        /// <summary>
        /// Get purchase order by Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<PurchaseOrder> GetPurchaseOrderByIdBackupInStoreAsync(Guid Id, Guid storeId)
        {
            var purchaseOrder = await dbSet.Where(m => m.Id == Id && m.StoreId == storeId)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(p => p.Material)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(m => m.Unit)
                .Include(p => p.PurchaseOrderMaterials).ThenInclude(m => m.Unit).ThenInclude(m => m.UnitConversions)
                .FirstOrDefaultAsync();
            return purchaseOrder;
        }

        public IQueryable<PurchaseOrder> GetAllPurchaseOrderByBranchAsync(Guid? storeId, Guid? branchId)
        {
            var allPurchaseOrder = dbSet.Where(b => b.StoreId == storeId && b.Id == branchId);

            return allPurchaseOrder;
        }
    }
}

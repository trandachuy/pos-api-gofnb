using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoFoodBeverage.Common.Extensions.PagingExtensions;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPurchaseOrderRepostiory : IGenericRepository<PurchaseOrder>
    {
        /// <summary>
        /// Get all purchase order by Store Id (pagingnation)
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="KeySearch"></param>
        /// <returns></returns>
        Task<Pager<PurchaseOrder>> GetAllPurchaseOrderByStoreId(Guid storeId, int pageNumber, int pageSize, string KeySearch);

        /// <summary>
        /// Get purchase order by Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<PurchaseOrder> GetPurchaseOrderByIdInStoreAsync(Guid Id, Guid storeId);

        /// <summary>
        /// Get all purchase order by material Id and store Id
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<IList<PurchaseOrder>> GetAllPurchaseOrderByMaterialIdInStoreAsync(Guid materialId, Guid storeId);

        /// <summary>
        /// Get all purchase order by supplier Id and store Id
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<IList<PurchaseOrder>> GetAllPurchaseOrderBySupplierIdInStoreAsync(Guid supplierId, Guid storeId);

        /// <summary>
        /// Get spurchase order by material Id and store Id
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<PurchaseOrder> GetPurchaseOrderByIdBackupInStoreAsync(Guid Id, Guid storeId);

        /// <summary>
        /// Get all PO by branch Id and store Id
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        IQueryable<PurchaseOrder> GetAllPurchaseOrderByBranchAsync(Guid? storeId, Guid? branchId);
    }
}

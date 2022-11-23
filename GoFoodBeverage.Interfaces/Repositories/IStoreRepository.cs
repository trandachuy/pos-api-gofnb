using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        Task<Store> GetStoreByIdWithoutTrackingAsync(Guid? storeId);

        Task<Store> GetStoreByIdAsync(Guid? storeId);

        Task<bool> IsStaffInitStoreAsync(Guid accountId, Guid storeId);

        Task<Store> GetStoreInformationByIdAsync(Guid? storeId);

        IQueryable<Store> GetStoresByAccountId(Guid? storeId);

        bool IsStaffInitStore(Guid accountId, Guid storeId);

        Task<Store> GetStoreAllBranchByStoreIdOrStoreBranchIdAsync(Guid? storeId, Guid? branchId);

        Task<Store> GetStoreKitchenConfigAsync(Guid? storeId);

        Task<Store> GetStoreConfigAsync(Guid? storeId);

        Task<Store> GetStoreConfigWithoutTrackingAsync(Guid? storeId);

        IQueryable<Store> GetStoresByStoreIds(IEnumerable<Guid> storeIds);
    }
}

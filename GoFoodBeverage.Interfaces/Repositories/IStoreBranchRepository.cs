using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStoreBranchRepository : IGenericRepository<StoreBranch>
    {
        IQueryable<StoreBranch> GetStoreBranchesByStoreId(Guid? storeId);

        IQueryable<StoreBranch> GetAllStoreBranches();

        IQueryable<StoreBranch> GetStoreBranchByIdAsync(Guid? branchId);

        IQueryable<StoreBranch> GetAnyStoreBranchByIdAsync(IEnumerable<Guid> branchIds);

        IQueryable<StoreBranch> GetStoreBranchByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId);

        IQueryable<StoreBranch> GetRemainingStoreBranchesByStoreId(Guid? storeId, IEnumerable<Guid?> branchIds);

        Task<StoreBranch> GetBranchByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId);

        Task<StoreBranch> GetBranchAddressByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId);

        IQueryable<StoreBranch> GetStoreBranchesByStoreIds(IEnumerable<Guid> storeIds);
    }
}

using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IMaterialInventoryBranchRepository : IGenericRepository<MaterialInventoryBranch>
    {
        /// <summary>
        /// Get aterialInventoryBranches by storeId and branchId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IQueryable<MaterialInventoryBranch> GetMaterialInventoryBranchesByBranchId(Guid storeId, Guid branchId);
    }
}

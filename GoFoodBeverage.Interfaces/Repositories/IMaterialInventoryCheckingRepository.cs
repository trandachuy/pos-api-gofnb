using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IMaterialInventoryCheckingRepository : IGenericRepository<MaterialInventoryChecking>
    {
        /// <summary>
        /// Get materialInventoryChecking by storeId and branchId
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        IQueryable<MaterialInventoryChecking> GetMaterialInventoryCheckingByShiftId(Guid storeId, Guid shiftId);
    }
}

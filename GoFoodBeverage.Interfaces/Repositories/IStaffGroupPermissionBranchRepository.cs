using System;
using System.Linq;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStaffGroupPermissionBranchRepository : IGenericRepository<StaffGroupPermissionBranch>
    {
        IQueryable<StaffGroupPermissionBranch> GetStaffGroupPermissionBranchesByStaffId(Guid staffId);

        IQueryable<StaffGroupPermissionBranch> GetStaffGroupPermissionBranchesByStaffIds(IEnumerable<Guid> staffIds, Guid? storeId);
    }
}

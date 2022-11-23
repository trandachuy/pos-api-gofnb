using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StaffGroupPermissionBranchRepository : GenericRepository<StaffGroupPermissionBranch>, IStaffGroupPermissionBranchRepository
    {
        public StaffGroupPermissionBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<StaffGroupPermissionBranch> GetStaffGroupPermissionBranchesByStaffId(Guid staffId)
        {
            var staffGroupPermissionBranches = dbSet
                .Where(s => s.StaffId == staffId)
                .Include(s => s.GroupPermissionBranches)
                .ThenInclude(g => g.StoreBranch)
                .Include(s => s.GroupPermissionBranches)
                .ThenInclude(g => g.GroupPermission);

            return staffGroupPermissionBranches;
        }

        public IQueryable<StaffGroupPermissionBranch> GetStaffGroupPermissionBranchesByStaffIds(IEnumerable<Guid> staffIds, Guid? storeId)
        {
            var staffGroupPermissionBranches = dbSet
                .Where(s => s.StoreId == storeId && staffIds.Any(sid => sid == s.StaffId))
                .Include(s => s.GroupPermissionBranches)
                .ThenInclude(g => g.StoreBranch)
                .Include(s => s.GroupPermissionBranches)
                .ThenInclude(g => g.GroupPermission);

            return staffGroupPermissionBranches;
        }
    }
}

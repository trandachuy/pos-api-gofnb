using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        public StaffRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }


        public IQueryable<Staff> GetAllStaffInStore(Guid storeId)
        {
            var staffs = dbSet.Where(s => s.StoreId == storeId && !s.IsDeleted);

            return staffs;
        }

        public Task<Staff> GetStaffByIdAsync(Guid accountId)
        {
            var staff = dbSet.Include(a => a.Store)
                .Include(a => a.Account)
                .FirstOrDefaultAsync(s => s.Id == accountId && !s.IsDeleted);

            return staff;
        }

        public Task<Staff> GetStaffByAccountIdAsync(Guid accountId)
        {
            var staff = dbSet.Include(s => s.Account).FirstOrDefaultAsync(s => s.AccountId == accountId && !s.IsDeleted);

            return staff;
        }

        public Task<Staff> GetStaffByIdForEditAsync(Guid staffId, Guid? storeId)
        {
            var staff = dbSet.Where(s => s.StoreId == storeId && s.Id == staffId && !s.IsDeleted)
                            .Include(s => s.StaffGroupPermissionBranchs)
                            .ThenInclude(p => p.GroupPermissionBranches)
                            .ThenInclude(p => p.StoreBranch)
                            .FirstOrDefaultAsync();

            return staff;
        }
        
        public Task<Staff> CheckExistStaffCodeInStoreAsync(Guid staffId, string staffCode, Guid storeId)
        {
            var staff = dbSet.FirstOrDefaultAsync(s => s.Id != staffId && s.Code.Trim().ToLower().Equals(staffCode.Trim().ToLower()) && s.StoreId == storeId);

            return staff;
        }

        public IQueryable<Staff> GetAllStaffByInitialStoreAccounts(List<Guid> initialStoreAccountIds)
        {
            var allStaffs = dbSet.Where(s => initialStoreAccountIds.Contains(s.AccountId) && !s.IsDeleted);

            return allStaffs;
        }

        public IQueryable<Staff> GetAllStaffByInitialStoreAccount(Guid storeAccountId)
        {
            var allStaffs = dbSet.Where(s => s.AccountId == storeAccountId && !s.IsDeleted);

            return allStaffs;
        }

        public IQueryable<Staff> GetAllStaffByListStaffId(List<Guid?> listStaffId)
        {
            var allStaffs = dbSet.Where(s => listStaffId.Any(Id => Id == s.Id) && !s.IsDeleted);

            return allStaffs;
        }

        public IQueryable<Staff> GetAllStaffByListStaff(IEnumerable<Guid> listStaffId, Guid? storeId)
        {
            var allStaffs = dbSet.Where(s => s.StoreId == storeId && listStaffId.Any(Id => Id == s.Id) && !s.IsDeleted);

            return allStaffs;
        }

        public IQueryable<Staff> GetStaffByShift(Shift shift, Guid? storeId)
        {
            var allStaffs = dbSet.Where(b => b.StoreId == storeId && shift.StaffId != null && (b.Id == shift.StaffId.Value) && !b.IsDeleted);

            return allStaffs;
        }

        public IQueryable<Staff> GetStaffById(Guid staffId, Guid? storeId)
        {
            var staff = dbSet.Where(m => m.StoreId == storeId && m.Id == staffId && !m.IsDeleted);

            return staff;
        }

        public IQueryable<Staff> GetStaffByAccountId(Guid accountId)
        {
            var staff = dbSet.Where(s => s.AccountId == accountId && !s.IsDeleted);

            return staff;
        }
    }
}

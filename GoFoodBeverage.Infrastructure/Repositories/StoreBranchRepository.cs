using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreBranchRepository : GenericRepository<StoreBranch>, IStoreBranchRepository
    {
        public StoreBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<StoreBranch> GetStoreBranchesByStoreId(Guid? storeId)
        {
            var branchManagements = dbSet.Where(b => b.StoreId == storeId && !b.IsDeleted);

            return branchManagements;
        }

        public IQueryable<StoreBranch> GetAllStoreBranches()
        {
            var allStoreBranches = dbSet.Where(b => !b.IsDeleted);

            return allStoreBranches;
        }

        public IQueryable<StoreBranch> GetStoreBranchByIdAsync(Guid? branchId)
        {
            var allStoreBranches = dbSet.Where(b => b.Id == branchId && !b.IsDeleted);

            return allStoreBranches;
        }

        public IQueryable<StoreBranch> GetAnyStoreBranchByIdAsync(IEnumerable<Guid> branchIds)
        {
            var allStoreBranches = dbSet.Where(p => branchIds.Any(id => id == p.Id) && !p.IsDeleted);

            return allStoreBranches;
        }

        public IQueryable<StoreBranch> GetStoreBranchByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId)
        {
            var allStoreBranches = dbSet.Where(b => b.StoreId == storeId && b.Id == branchId && !b.IsDeleted);

            return allStoreBranches;
        }

        public IQueryable<StoreBranch> GetRemainingStoreBranchesByStoreId(Guid? storeId, IEnumerable<Guid?> branchIds)
        {
            var allStoreBranches = dbSet.Where(x => !branchIds.Contains(x.Id) && x.StoreId == storeId && !x.IsDeleted);

            return allStoreBranches;
        }

        public async Task<StoreBranch> GetBranchByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId)
        {
            var branchInfo = await dbSet
                .Where(b => b.StoreId == storeId && b.Id == branchId && !b.IsDeleted)
                .Include(b => b.Store)
                .FirstOrDefaultAsync();

            return branchInfo;
        }

        public async Task<StoreBranch> GetBranchAddressByStoreIdAndBranchIdAsync(Guid? storeId, Guid? branchId)
        {
            var branchInfo = await dbSet
                .Where(b => b.StoreId == storeId && b.Id == branchId && !b.IsDeleted)
                .Include(o => o.Address).ThenInclude(a => a.City)
                .Include(o => o.Address).ThenInclude(a => a.District)
                .Include(o => o.Address).ThenInclude(a => a.Ward)
                .Include(o => o.Address).ThenInclude(a => a.State)
                .Include(o => o.Address).ThenInclude(a => a.Country)
                .Include(b => b.Store)
                .FirstOrDefaultAsync();

            return branchInfo;
        }

        public IQueryable<StoreBranch> GetStoreBranchesByStoreIds(IEnumerable<Guid> storeIds)
        {
            var storeBranches = _dbContext.StoreBranches.Where(o => storeIds.Contains(o.StoreId));

            return storeBranches;
        }
    }
}

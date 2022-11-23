using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public Task<Store> GetStoreByIdWithoutTrackingAsync(Guid? storeId)
        {
            var store = dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == storeId);

            return store;
        }

        public Task<Store> GetStoreByIdAsync(Guid? storeId)
        {
            var store = dbSet.Include(o => o.Currency).FirstOrDefaultAsync(s => s.Id == storeId);

            return store;
        }

        public async Task<bool> IsStaffInitStoreAsync(Guid accountId, Guid storeId)
        {
            var store = await dbSet.FirstOrDefaultAsync(s => s.Id == storeId && s.InitialStoreAccountId == accountId);

            return store != null;
        }

        public Task<Store> GetStoreInformationByIdAsync(Guid? storeId)
        {
            var store = dbSet
                .Include(o => o.Currency)
                .Include(o => o.Address).ThenInclude(a => a.City)
                .Include(o => o.Address).ThenInclude(a => a.District)
                .Include(o => o.Address).ThenInclude(a => a.Ward)   
                .Include(o => o.Address).ThenInclude(a => a.State)
                .Include(o => o.Address).ThenInclude(a => a.Country)
                .FirstOrDefaultAsync(s => s.Id == storeId);

            return store;
        }

        public IQueryable<Store> GetStoresByAccountId(Guid? accountId)
        {
            var stores = dbSet.Where(b => b.InitialStoreAccountId == accountId);

            return stores;
        }

        public bool IsStaffInitStore(Guid accountId, Guid storeId)
        {
            var store = dbSet.FirstOrDefault(s => s.Id == storeId && s.InitialStoreAccountId == accountId);

            return store != null;
        }

        public async Task<Store> GetStoreAllBranchByStoreIdOrStoreBranchIdAsync(Guid? storeId, Guid? branchId)
        {
            Store storeEntity = new Store();

            if (branchId.HasValue && !storeId.HasValue)
            {
                storeId = await _dbContext.StoreBranches
                    .Where(a => a.Id == branchId && a.StatusId == (int)EnumStatus.Active).Select(a => a.StoreId)
                    .FirstOrDefaultAsync();
            }

            storeEntity = await _dbContext.Stores.Where(a => a.Id == storeId)
                .Include(a => a.StoreBranches.Where(a => a.StatusId == (int)EnumStatus.Active)).ThenInclude(a => a.Address).ThenInclude(a => a.Ward)
                .Include(a => a.StoreBranches.Where(a => a.StatusId == (int)EnumStatus.Active)).ThenInclude(a => a.Address).ThenInclude(a => a.District)
                .Include(a => a.StoreBranches.Where(a => a.StatusId == (int)EnumStatus.Active)).ThenInclude(a => a.Address).ThenInclude(a => a.City)
                .Include(a => a.StoreBranches.Where(a => a.StatusId == (int)EnumStatus.Active)).ThenInclude(a => a.Address).ThenInclude(a => a.Country)
                .Include(a => a.Address).ThenInclude(a => a.Ward)
                .Include(a => a.Address).ThenInclude(a => a.District)
                .Include(a => a.Address).ThenInclude(a => a.City)
                .Include(a => a.Address).ThenInclude(a => a.Country)
                .AsNoTracking()
                .Select(a => new Store()
                {
                    Id = a.Id,
                    Address = a.Address,
                    AddressId = a.AddressId,
                    Title = a.Title,
                    StoreBranches = a.StoreBranches
                })
                .FirstOrDefaultAsync();

            return storeEntity;
        }

        public Task<Store> GetStoreKitchenConfigAsync(Guid? storeId)
        {
            var store = dbSet.Where(s => s.Id == storeId)
                .Select(s => new Store
                {
                    Id = s.Id,
                    IsStoreHasKitchen = s.IsStoreHasKitchen,
                    IsAutoPrintStamp = s.IsAutoPrintStamp,
                }).FirstOrDefaultAsync();

            return store;
        }

        public Task<Store> GetStoreConfigAsync(Guid? storeId)
        {
            var store = dbSet.Where(s => s.Id == storeId)
                .Select(s => new Store
                {
                    Id = s.Id,
                    IsStoreHasKitchen = s.IsStoreHasKitchen,
                    IsAutoPrintStamp = s.IsAutoPrintStamp,
                    IsPaymentLater = s.IsPaymentLater,
                    IsCheckProductSell = s.IsCheckProductSell,
                })
                .FirstOrDefaultAsync();

            return store;
        }

        public Task<Store> GetStoreConfigWithoutTrackingAsync(Guid? storeId)
        {
            var store = dbSet.Where(s => s.Id == storeId)
                .Select(s => new Store
                {
                    Id = s.Id,
                    IsStoreHasKitchen = s.IsStoreHasKitchen,
                    IsAutoPrintStamp = s.IsAutoPrintStamp,
                    IsPaymentLater = s.IsPaymentLater,
                    IsCheckProductSell = s.IsCheckProductSell,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return store;
        }

        public IQueryable<Store> GetStoresByStoreIds(IEnumerable<Guid> storeIds)
        {
            var stores = _dbContext.Stores.Where(o => storeIds.Contains(o.Id));

            return stores;
        }
    }
}

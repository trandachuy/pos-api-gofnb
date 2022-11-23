using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboRepository : GenericRepository<Combo>, IComboRepository
    {
        public ComboRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Combo> GetAllCombosInStore(Guid storeId)
        {
            var query = dbSet.Where(c => c.StoreId == storeId);

            return query;
        }

        public IQueryable<Combo> GetAllCombosInStoreActivies(Guid storeId)
        {
            var now = DateTime.UtcNow.Date;
            var allCombos = GetAllCombosInStore(storeId);
            var query = from combo in allCombos
                        where combo.IsStopped != true
                            && (combo.EndDate == null || combo.EndDate.Value.Date >= now)
                        select combo;

            return query;
        }

        public IQueryable<Combo> GetAllCombosCanDisplay(Guid storeId)
        {
            var now = DateTime.UtcNow.Date;
            var query = from combo in GetAllCombosInStoreActivies(storeId)
                        where (combo.StartDate.Date <= now)
                        select combo;

            return query;
        }

        public IQueryable<Combo> GetAllCombosInStoreInclude(Guid storeId)
        {
            var query = GetAllCombosCanDisplay(storeId)
                .Include(c => c.ComboProductPrices)
                    .ThenInclude(pr => pr.ProductPrice)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(a => a.ProductOptions)
                    .ThenInclude(a => a.Option)
                    .ThenInclude(a => a.OptionLevel)
                .Include(c => c.ComboPricings.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cp => cp.ComboPricingProducts)
                    .ThenInclude(cpp => cpp.ProductPrice)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(a => a.ProductOptions)
                    .ThenInclude(a => a.Option)
                    .ThenInclude(a => a.OptionLevel)
                .Include(c => c.ComboProductGroups.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cpg => cpg.ComboProductGroupProductPrices.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cpgpr => cpgpr.ProductPrice)
                    .ThenInclude(pr => pr.Product);

            return query;
        }

        public async Task<Combo> GetComboByIdAsync(Guid comboId, Guid? storeId)
        {
            var combo = await dbSet
                .Where(m => m.StoreId == storeId && m.Id == comboId)
                .Include(c => c.ComboStoreBranches).ThenInclude(csb => csb.Branch)
                .Include(c => c.ComboProductPrices).ThenInclude(pr => pr.ProductPrice).ThenInclude(p => p.Product)
                .Include(c => c.ComboPricings.OrderBy(cp => cp.CreatedTime)).ThenInclude(cp => cp.ComboPricingProducts).ThenInclude(cpp => cpp.ProductPrice).ThenInclude(p => p.Product)
                .Include(c => c.ComboProductGroups).ThenInclude(cpg => cpg.ComboProductGroupProductPrices).ThenInclude(cpgpr => cpgpr.ProductPrice).ThenInclude(pr => pr.Product)
                .Include(c => c.ComboProductGroups).ThenInclude(cpg => cpg.ProductCategory)
                .FirstOrDefaultAsync();

            return combo;
        }

        public IQueryable<Combo> GetComboByIdWithoutTracking(Guid storeId, Guid comboId)
        {
            var combo = dbSet
                .Where(m => m.StoreId == storeId && m.Id == comboId)
                .AsNoTracking()
                .Include(c => c.ComboProductPrices)
                    .ThenInclude(pr => pr.ProductPrice)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(a => a.ProductOptions)
                    .ThenInclude(a => a.Option)
                    .ThenInclude(a => a.OptionLevel)
                .Include(c => c.ComboPricings.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cp => cp.ComboPricingProducts)
                    .ThenInclude(cpp => cpp.ProductPrice)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(a => a.ProductOptions)
                    .ThenInclude(a => a.Option)
                    .ThenInclude(a => a.OptionLevel)
                .Include(c => c.ComboProductGroups.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cpg => cpg.ComboProductGroupProductPrices.OrderBy(cp => cp.CreatedTime))
                    .ThenInclude(cpgpr => cpgpr.ProductPrice)
                    .ThenInclude(pr => pr.Product)
                    .ThenInclude(a => a.ProductOptions)
                    .ThenInclude(a => a.Option)
                    .ThenInclude(a => a.OptionLevel)
                .Include(c => c.ComboProductGroups)
                    .ThenInclude(cpg => cpg.ProductCategory);

            return combo;
        }
    }
}

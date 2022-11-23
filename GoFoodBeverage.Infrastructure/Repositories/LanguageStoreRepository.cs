using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Language.Dto;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class LanguageStoreRepository : GenericRepository<LanguageStore>, ILanguageStoreRepository
    {
        public LanguageStoreRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<IList<LanguageStoreDto>> GetLanguageStoreByStoreId(Guid? storeId)
        {
            var language = _dbContext
                .Languages
                .Include(l => l.LanguageStores)
                .Where(l => l.IsDefault == true)
                .Select(l => new LanguageStoreDto { Name = l.Name, Emoji = l.Emoji, IsPublish = true, IsDefault = true, Id = Guid.Empty });

            var languageStore = await dbSet
                .Include(ls => ls.Language)
                .Where(ls => ls.StoreId == storeId)
                .Join(
                    _dbContext.Languages,
                    ls => ls.LanguageId,
                    l => l.Id,
                    (ls, l) => new LanguageStoreDto { Name = l.Name, Emoji = l.Emoji, IsPublish = !ls.IsPublish, IsDefault = false, Id = ls.Id }
                )
                .Union(language)
                .OrderByDescending(ls => ls.IsDefault == true)
                .ToListAsync();

            return languageStore;
        }

        public async Task<LanguageStore> GetLanguageStoreById(Guid? id)
        {
            LanguageStore languageStore = await dbSet.FirstOrDefaultAsync(ls => ls.Id == id);

            return languageStore;
        }

        public async Task<List<LanguageStoreDto>> GetLanguageByStoreIdAndIsPublish(Guid? storeId)
        {
            var language = _dbContext
                .Languages
                .Where(l => l.IsDefault == true)
                .Select(l => new LanguageStoreDto { Name = l.Name, Emoji = l.Emoji, LanguageCode = l.LanguageCode, CountryCode = l.CountryCode, IsDefault = true });

            var languageStore = await dbSet
                .Include(ls => ls.Language)
                .Where(ls => ls.StoreId == storeId && ls.IsPublish == true)
                .Join(
                    _dbContext.Languages,
                    ls => ls.LanguageId,
                    l => l.Id,
                    (ls, l) => new LanguageStoreDto { Name = l.Name, Emoji = l.Emoji, LanguageCode = l.LanguageCode, CountryCode = l.CountryCode, IsDefault = false }
                 ).Union(language)
                 .OrderByDescending(ls => ls.IsDefault == true)
                 .ToListAsync();

            return languageStore;
        }
    }
}

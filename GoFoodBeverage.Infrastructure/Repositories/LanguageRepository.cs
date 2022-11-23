using System.Linq;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Language> GetAllLanguagesNotApplyInStore(Guid? storeId)
        {
            var languages = dbSet
                .Where(lang =>
                !lang.IsDefault &&
                !_dbContext
                   .LanguageStores
                   .Where(ls => ls.StoreId == storeId)
                   .Select(ls => ls.LanguageId)
                   .Any(languageId => languageId == lang.Id)
                );

            return languages;
        }

        public Language GetLanguageById(Guid? id)
        {
            var language = dbSet.FirstOrDefault(l => l.Id == id);

            return language;
        }
    }
}

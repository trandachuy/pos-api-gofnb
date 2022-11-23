using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        IQueryable<Language> GetAllLanguagesNotApplyInStore(Guid? storeId);

        Language GetLanguageById(Guid? id);
    }
}

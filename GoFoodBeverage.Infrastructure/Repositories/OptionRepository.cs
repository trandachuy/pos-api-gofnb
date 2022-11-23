using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OptionRepository : GenericRepository<Option>, IOptionRepository
    {
        public OptionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Option> GetAllOptionsInStore(Guid? storeId)
        {
            var options = dbSet
                .Where(o => o.StoreId == storeId)
                .Include(o => o.Store)
                .Include(o => o.OptionLevel);

            return options;
        }

        public Task<Option> GetOptionDetailByIdAsync(Guid optionId, Guid? storeId)
        {
            var option = dbSet
                .Where(o => o.StoreId == storeId && o.Id == optionId)
                .Include(o => o.Material)
                .Include(o => o.OptionLevel)
                .FirstOrDefaultAsync();

            return option;
        }

        public Task<Option> CheckExistOptionNameInStoreAsync(Guid optionId, string optionName, Guid storeId)
        {
            var option = dbSet.FirstOrDefaultAsync(o => o.Id != optionId && o.Name.Trim().ToLower().Equals(optionName.Trim().ToLower()) && o.StoreId == storeId);

            return option;
        }
    }
}

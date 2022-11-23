using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PageRepository : GenericRepository<Page>, IPageRepository
    {
        public PageRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Page> GetAllPagesInStore(Guid? storeId)
        {
            var pages = dbSet.Where(s => s.StoreId == storeId && s.IsActive == true);

            return pages;
        }

        public async Task<Page> GetPageByIdInStoreAsync(Guid pageId, Guid storeId)
        {
            var page = await dbSet.Where(s => s.Id == pageId && s.StoreId == storeId && s.IsActive == true)
                .FirstOrDefaultAsync();

            return page;
        }
    }
}

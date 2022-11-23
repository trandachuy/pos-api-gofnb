using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OnlineStoreMenuRepository : GenericRepository<OnlineStoreMenu>, IOnlineStoreMenuRepostiory
    {
        public OnlineStoreMenuRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<OnlineStoreMenu> GetAllMenuInStore(Guid storeId)
        {
            var menus = dbSet.Where(s => s.StoreId == storeId).Include(o => o.OnlineStoreMenuItems);

            return menus;
        }

        public IQueryable<OnlineStoreMenu> GetAllSubMenuInStore(Guid? storeId)
        {
            var menus = dbSet
                .Where(m => m.StoreId == storeId && m.Level == EnumLevelMenu.Level2)
                .Include(m => m.OnlineStoreMenuItems.OrderBy(x => x.Position));

            return menus;
        }

        public Task<OnlineStoreMenu> GetMenuNameInStoreAsync(string menuName, Guid? storeId)
        {
            var menu = dbSet.FirstOrDefaultAsync(p => p.StoreId == storeId && p.Name.Trim().ToLower().Equals(menuName.Trim().ToLower()));

            return menu;
        }
    }
}

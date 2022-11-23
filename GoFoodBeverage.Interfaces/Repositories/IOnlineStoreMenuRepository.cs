using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOnlineStoreMenuRepostiory : IGenericRepository<OnlineStoreMenu>
    {
        IQueryable<OnlineStoreMenu> GetAllMenuInStore(Guid storeId);

        IQueryable<OnlineStoreMenu> GetAllSubMenuInStore(Guid? storeId);

        Task<OnlineStoreMenu> GetMenuNameInStoreAsync(string menuName, Guid? storeId);
    }
}

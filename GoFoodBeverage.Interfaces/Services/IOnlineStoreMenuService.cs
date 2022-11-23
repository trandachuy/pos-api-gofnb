using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Services
{
    public interface IOnlineStoreMenuService
    {
        Task<bool> CreateDataMenuDefaultAsync(Guid storeId);
    }
}

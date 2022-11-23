using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IFeeRepository : IGenericRepository<Fee>
    {
        IQueryable<Fee> GetAllFeesInStore(Guid? storeId);

        Task<Fee> GetFeeByIdInStoreAsync(Guid id, Guid storeId);

        IQueryable<Fee> GetAllFeesByListIdInStore(IQueryable<Guid> listFeeIds, Guid? storeId);

        IQueryable<Fee> GetFeesForCreateOrder(Guid? storeId, IEnumerable<Guid> orderFeeIds);
    }
}

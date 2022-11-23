using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GoFoodBeverage.Common.Extensions.PagingExtensions;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ITaxRepository : IGenericRepository<Tax>
    {
        IQueryable<Tax> GetAllTaxInStore(Guid storeId);

        /// <summary>
        /// Get tax by Id, store Id and branchId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        IQueryable<Tax> GetTaxById(Guid id, Guid? storeId);
    }
}

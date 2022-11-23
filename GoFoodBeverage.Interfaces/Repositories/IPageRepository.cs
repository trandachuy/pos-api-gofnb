using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPageRepository : IGenericRepository<Page>
    {
        IQueryable<Page> GetAllPagesInStore(Guid? storeId);

        Task<Page> GetPageByIdInStoreAsync(Guid pageId, Guid storeId);
    }
}

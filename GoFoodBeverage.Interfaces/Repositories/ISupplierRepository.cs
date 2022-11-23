using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        IQueryable<Supplier> GetAllSuppliersInStore(Guid? storeId);

        Task<Supplier> GetSupplierByIdInStoreAsync(Guid id, Guid storeId);
    }
}

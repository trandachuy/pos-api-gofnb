using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        IQueryable<Unit> GetAllUnitsInStore(Guid? storeId);
    }
}

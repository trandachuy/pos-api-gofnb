using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IUnitConversionRepository : IGenericRepository<UnitConversion>
    {
        IQueryable<UnitConversion> GetAllUnitConversionsInStore(Guid? storeId);

        IQueryable<UnitConversion> GetUnitConversionsByMaterialIdInStore(Guid materialId, Guid? storeId);
    }
}

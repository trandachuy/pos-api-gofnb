using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IDistrictRepository : IGenericRepository<District>
    {
        IQueryable<District> GetDistrictsByCityId(Guid cityId);
    }
}

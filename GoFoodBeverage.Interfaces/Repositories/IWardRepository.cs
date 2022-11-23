using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IWardRepository : IGenericRepository<Ward>
    {
        IQueryable<Ward> GetWardsByDistrictId(Guid districtId);
    }
}

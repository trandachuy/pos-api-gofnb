using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class WardRepository : GenericRepository<Ward>, IWardRepository
    {
        public WardRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Ward> GetWardsByDistrictId(Guid districtId)
        {
            IQueryable<Ward> wards = dbSet.Where(c => c.DistrictId == districtId);

            return wards;
        }
    }
}

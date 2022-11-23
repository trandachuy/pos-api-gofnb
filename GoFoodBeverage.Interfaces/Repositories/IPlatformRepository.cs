using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPlatformRepository : IGenericRepository<Platform>
    {
        IQueryable<Platform> GetActivePlatforms();
    }
}

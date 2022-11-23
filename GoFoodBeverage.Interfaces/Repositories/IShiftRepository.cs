using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        IQueryable<Shift> GetAllShiftInBranch(Guid branchId);
    }
}

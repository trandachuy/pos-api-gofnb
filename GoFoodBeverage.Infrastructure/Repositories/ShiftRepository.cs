using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Shift> GetAllShiftInBranch(Guid branchId)
        {
            var shifts = dbSet.Where(s => s.BranchId == branchId);

            return shifts;
        }
    }
}

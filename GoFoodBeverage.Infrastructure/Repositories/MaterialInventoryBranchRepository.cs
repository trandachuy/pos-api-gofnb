using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class MaterialInventoryBranchRepository : GenericRepository<MaterialInventoryBranch>, IMaterialInventoryBranchRepository
    {
        public MaterialInventoryBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<MaterialInventoryBranch> GetMaterialInventoryBranchesByBranchId(Guid storeId, Guid branchId)
        {
            var materialInventoryBranches = dbSet.Where(m => m.StoreId == storeId && m.BranchId == branchId).Include(p => p.Material).ThenInclude(u => u.Unit);

            return materialInventoryBranches;
        }
    }
}

﻿using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FeeBranchRepository : GenericRepository<FeeBranch>, IFeeBranchRepository
    {
        public FeeBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

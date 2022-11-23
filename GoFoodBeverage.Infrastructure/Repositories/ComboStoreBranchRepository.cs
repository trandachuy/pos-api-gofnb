using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboStoreBranchRepository : GenericRepository<ComboStoreBranch>, IComboStoreBrancheRepository
    {
        public ComboStoreBranchRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

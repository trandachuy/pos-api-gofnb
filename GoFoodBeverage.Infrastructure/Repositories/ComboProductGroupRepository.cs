using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboProductGroupRepository : GenericRepository<ComboProductGroup>, IComboProductGroupRepository
    {
        public ComboProductGroupRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

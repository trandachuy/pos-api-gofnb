using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class InternalAccountRepository : GenericRepository<InternalAccount>, IInternalAccountRepository
    {
        public InternalAccountRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

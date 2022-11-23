using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AccountTransferRepository : GenericRepository<AccountTransfer>, IAccountTransferRepository
    {
        public AccountTransferRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

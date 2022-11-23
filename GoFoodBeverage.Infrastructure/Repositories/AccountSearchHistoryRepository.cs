using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class AccountSearchHistoryRepository : GenericRepository<AccountSearchHistory>, IAccountSearchHistoryRepository
    {
        public AccountSearchHistoryRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}
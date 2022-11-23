using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboProductPriceRepository : GenericRepository<ComboProductPrice>, IComboProductPriceRepository
    {
        public ComboProductPriceRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

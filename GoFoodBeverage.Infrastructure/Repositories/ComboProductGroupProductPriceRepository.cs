using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Application.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class ComboProductGroupProductPriceRepository : GenericRepository<ComboProductGroupProductPrice>, IComboProductGroupProductPriceRepository
    {
        public ComboProductGroupProductPriceRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

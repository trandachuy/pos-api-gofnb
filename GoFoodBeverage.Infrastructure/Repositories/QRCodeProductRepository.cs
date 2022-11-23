using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class QRCodeProductRepository : GenericRepository<QRCodeProduct>, IQRCodeProductRepostiory
    {
        public QRCodeProductRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}

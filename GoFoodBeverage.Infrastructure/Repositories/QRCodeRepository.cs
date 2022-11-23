using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class QRCodeRepository : GenericRepository<QRCode>, IQRCodeRepostiory
    {
        public QRCodeRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<QRCode> GetAllQRCodeInStore(Guid storeId)
        {
            var qrCodes = dbSet.Where(s => s.StoreId == storeId);

            return qrCodes;
        }

        public async Task<QRCode> GetQRCodeByIdAsync(Guid qrCodeId, Guid? storeId)
        {
            var combo = await dbSet
                .Where(x => x.StoreId == storeId && x.Id == qrCodeId)
                .Include(x => x.QRCodeProducts).ThenInclude(p => p.Product).ThenInclude(u => u.Unit)
                .FirstOrDefaultAsync();

            return combo;
        }
    }
}

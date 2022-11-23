using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PaymentConfigRepository : GenericRepository<PaymentConfig>, IPaymentConfigRepository
    {
        public PaymentConfigRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<PaymentConfig> GetAllPaymentConfigsInStore(Guid? storeId)
        {
            var paymentConfigs = dbSet.Where(o => o.StoreId == storeId);

            return paymentConfigs;
        }

        public async Task<PaymentConfig> GetPaymentConfigAsync(Guid? storeId, EnumPaymentMethod paymentMethod)
        {
            var paymentConfig = await dbSet
                .Where(o => o.StoreId == storeId && o.PaymentMethodEnumId == paymentMethod)
                .FirstOrDefaultAsync();

            return paymentConfig;
        }

        public Task<bool> IsActiveAsync(Guid? storeId, EnumPaymentMethod paymentMethod)
        {
            var paymentConfig = dbSet.AnyAsync(p => p.StoreId == storeId && p.PaymentMethodEnumId == paymentMethod && p.IsActivated);

            return paymentConfig;
        }
    }
}

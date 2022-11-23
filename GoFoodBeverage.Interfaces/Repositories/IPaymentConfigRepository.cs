using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPaymentConfigRepository : IGenericRepository<PaymentConfig>
    {
        IQueryable<PaymentConfig> GetAllPaymentConfigsInStore(Guid? storeId);

        Task<PaymentConfig> GetPaymentConfigAsync(Guid? storeId, EnumPaymentMethod paymentMethod);

        Task<bool> IsActiveAsync(Guid? storeId, EnumPaymentMethod paymentMethod);
    }
}

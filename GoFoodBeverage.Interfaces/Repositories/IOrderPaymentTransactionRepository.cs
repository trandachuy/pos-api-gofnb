using GoFoodBeverage.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOrderPaymentTransactionRepository : IGenericRepository<OrderPaymentTransaction>
    {
        Task<OrderPaymentTransaction> GetOrderPaymentTransactionById(Guid? id);

        Task<OrderPaymentTransaction> GetPaymentTransactionByOrderId(Guid? orderId);
    }
}

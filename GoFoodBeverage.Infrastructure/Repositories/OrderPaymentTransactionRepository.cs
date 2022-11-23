using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderPaymentTransactionRepository : GenericRepository<OrderPaymentTransaction>, IOrderPaymentTransactionRepository
    {
        public OrderPaymentTransactionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<OrderPaymentTransaction> GetOrderPaymentTransactionById(Guid? id)
        {
            var orderPaymentTransaction = await dbSet.Where(o => o.Id == id).FirstOrDefaultAsync();

            return orderPaymentTransaction;
        }

        public async Task<OrderPaymentTransaction> GetPaymentTransactionByOrderId(Guid? orderId)
        {
            var aPaymentTransaction = await dbSet.SingleOrDefaultAsync(transaction => transaction.OrderId == orderId);

            return aPaymentTransaction;
        }
    }
}

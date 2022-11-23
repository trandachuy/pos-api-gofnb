using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderSessionRepository : GenericRepository<OrderSession>, IOrderSessionRepository
    {
        public OrderSessionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<OrderSession> GetKitchenOrderSessionInStore(Guid? storeId, Guid? branchId)
        {
            var orderSessions = dbSet
                .Where(o => o.Order.StoreId == storeId && o.Order.BranchId == branchId)
                .Include(o => o.Order).ThenInclude(o => o.AreaTable).ThenInclude(o => o.Area)
                .Include(o => o.OrderItems).ThenInclude(o => o.OrderItemOptions)
                .Include(o => o.OrderItems).ThenInclude(o => o.OrderItemToppings)
                .Include(o => o.OrderItems).ThenInclude(o => o.ProductPrice)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                // Load product.
                .ThenInclude(ocpi => ocpi.ProductPrice)
                .ThenInclude(pp => pp.Product)

                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                // Load options.
                .ThenInclude(ocpi => ocpi.OrderItemOptions)

                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                // Load toppings.
                .ThenInclude(ocpi => ocpi.OrderItemToppings);

            return orderSessions;
        }

        public Task<OrderSession> GetOrderSessionByIdAsync(Guid orderSessionId, Guid? storeId)
        {
            var orderSession = dbSet.Where(o => o.StoreId == storeId && o.Id == orderSessionId).Include(o => o.Order).FirstOrDefaultAsync();
            return orderSession;
        }
    }
}

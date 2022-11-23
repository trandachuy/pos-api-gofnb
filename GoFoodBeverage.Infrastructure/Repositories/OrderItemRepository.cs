using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public async Task<IList<OrderItem>> GetOrderItemByOrderIdAsync(Guid? orderId)
        {
            var orderItems = await dbSet
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.ProductPrice).ThenInclude(pp => pp.Product)
                .Include(oi => oi.OrderItemOptions)
                .Include(oi => oi.OrderItemToppings)

                .Include(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.ProductPrice)
                .ThenInclude(pp => pp.Product)

                .Include(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemOptions)

                .Include(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemToppings)
                .AsNoTracking()

                .ToListAsync();

            return orderItems;
        }

        public async Task<List<OrderItem>> GetOrderItemByProductPriceIdAsync(Guid? productPriceId)
        {
            var orderItems = await dbSet
                .Where(oi => oi.ProductPriceId == productPriceId)
                .Include(oi => oi.Order)
                .OrderBy(oi => oi.CreatedTime)
                .ToListAsync();

            return orderItems;
        }

        public async Task<List<OrderItem>> GetOrderItemByOrderSessionIdAsync(Guid? orderSessionId, Guid? storeId)
        {
            var orderItems = await dbSet
                .Where(o => o.StoreId == storeId && o.OrderSessionId == orderSessionId && o.StatusId != EnumOrderItemStatus.Canceled)
                .Include(o => o.OrderComboItem).ThenInclude(o => o.OrderComboProductPriceItems)
                .ToListAsync();

            return orderItems;
        }

        public Task<OrderItem> GetOrderItemForUpdateStatusAsync(Guid orderItemId, Guid sessionId, Guid productId, DateTime? createdTime, Guid? storeId)
        {
            var orderSession = dbSet.Where(o =>
                o.StoreId == storeId
                && (o.Id == orderItemId || o.ProductId == productId)
                && o.OrderSessionId == sessionId
                && o.CreatedTime.Value.Date == createdTime.Value.Date
                && o.CreatedTime.Value.Hour == createdTime.Value.Hour
                && o.CreatedTime.Value.Minute == createdTime.Value.Minute
                && o.StatusId == EnumOrderItemStatus.New).FirstOrDefaultAsync();

            return orderSession;
        }

        public async Task<EnumOrderItemStatus> GetOrderItemStatusByIdWithoutTrackingAsync(Guid id)
        {
            return await dbSet.Where(a => a.Id == id)
                .AsNoTracking()
                .Select(a => a.StatusId)
                .SingleOrDefaultAsync();
        }
    }
}

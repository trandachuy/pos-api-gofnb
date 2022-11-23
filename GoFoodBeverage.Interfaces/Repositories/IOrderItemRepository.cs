using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Kitchen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<IList<OrderItem>> GetOrderItemByOrderIdAsync(Guid? orderId);

        Task<List<OrderItem>> GetOrderItemByProductPriceIdAsync(Guid? productPriceId);

        Task<List<OrderItem>> GetOrderItemByOrderSessionIdAsync(Guid? orderSessionId, Guid? storeId);

        /// <summary>
        /// Get order item for update status in POS Kitchen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OrderItem> GetOrderItemForUpdateStatusAsync(Guid orderItemId, Guid sessionId, Guid productId, DateTime? createdTime, Guid? storeId);

        Task<EnumOrderItemStatus> GetOrderItemStatusByIdWithoutTrackingAsync(Guid id);

    }
}

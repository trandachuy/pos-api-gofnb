using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.POS.Models.Fee;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.POS
{
    public interface IOrderService
    {
        decimal CalculateTotalFeeValue(decimal originalPrice, IEnumerable<Fee> fees);

        decimal CalculateTotalFeeValue(decimal originalPrice, IEnumerable<FeeModel> fees);

        List<Tuple<OrderItem, List<OrderItem>>> MergeSameOrderItems(List<OrderItem> orderItems);

        /// <summary>
        /// Calculate total cost of all product items an order. 
        /// ProductPrices should be include ProductPriceMaterials > material
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="productPrices"></param>
        /// <returns></returns>
        Task<decimal> CalculateTotalProductCostAsync(IEnumerable<ProductCartItemModel> cartItems, List<ProductPrice> productPrices);

        Task CloneOrderDetailAsync(Domain.Entities.Order order, Guid? storeId);

        Task<PosOrderDetailFromRestoreModel> GetOrderDetailDataFromRestoreByOrderIdAsync(Guid orderId);

        Task<bool> SaveOrderHistoryAsync(Guid OrderId, string OldOrder, string NewOrder, string ActionName, string Note, string Reason);

        Task<bool> CalMaterialQuantity(Guid orderId, bool refundQuantity, bool isHaveTransaction, EnumInventoryHistoryAction action);

        Task<OrderStatusModel> GetOrderStatusAsync(EnumOrderType orderType, EnumDeliveryMethod? deliveryMethod);

        Task<OrderStatusModel> GetOrderStatusAsync(
            Guid storeId,
            EnumOrderType orderType,
            EnumDeliveryMethod? deliveryMethod
        );

        Task<bool> CalculatePoint(Domain.Entities.Order order, Guid? storeId);
    }
}

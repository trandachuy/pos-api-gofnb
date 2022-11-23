using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        IQueryable<Order> GetAllOrdersInStore(Guid? storeId);

        IQueryable<Order> GetOrdersByBranchIdInStore(Guid? storeId, Guid? branchId);

        Task<Order> GetOrderByIdInStoreAsync(Guid? id, Guid? storeId);

        IQueryable<Order> GetOrdersNotCancleByShiftInStore(Guid? storeId, Guid? branchId);

        Task<Order> GetOrderByIdAsync(Guid? storeId, Guid? branchId, Guid? orderId);

        Task<Order> GetOrderByIdAsync(Guid? storeId, Guid? orderId);

        /// <summary>
        /// This method is used to get the current order by order code.
        /// </summary>
        /// <param name="code">For example: 770</param>
        /// <returns></returns>
        Task<Order> GetOrderByCode(string code);

        IQueryable<Order> CountOrderByStoreBranchId(Guid? storeId, Guid? branchId);

        /// <summary>
        /// This method is used to get the user's orders.
        /// </summary>
        /// <param name="customerId">For example: b3b849b4-2344-4755-ac53-03ce0df246ee</param>
        /// <returns></returns>
        IQueryable<Order> GetOrderListByCustomerId(Guid? customerId);

        Task<Order> GetOrderDetailByIdAsync(Guid customerId, Guid orderId);

        IQueryable<Order> GetOrderDetailDataById(Guid? id, Guid? storeId);
    }
}

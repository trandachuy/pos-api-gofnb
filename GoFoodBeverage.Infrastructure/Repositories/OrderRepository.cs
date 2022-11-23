using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<Order> GetAllOrdersInStore(Guid? storeId)
        {
            var ordersManagement = dbSet.Where(o => o.StoreId == storeId);
            return ordersManagement;
        }

        public async Task<Order> GetOrderByIdInStoreAsync(Guid? id, Guid? storeId)
        {
            var order = await dbSet
                .Where(o => o.Id == id && o.StoreId == storeId)
                .Include(o => o.Customer).ThenInclude(c => c.CustomerPoint)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.City)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.District)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.Ward)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.State)
                .FirstOrDefaultAsync();

            return order;
        }

        public IQueryable<Order> GetOrderDetailDataById(Guid? id, Guid? storeId)
        {
            var order = dbSet.Where(o => o.Id == id && o.StoreId == storeId)
                .Include(o => o.Customer).ThenInclude(c => c.CustomerPoint)
                .Include(of => of.OrderFees)
                .Include(o => o.OrderDelivery)

                .Include(o => o.Customer).ThenInclude(c => c.CustomerPoint)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.City)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.District)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.Ward)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.State)

                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductPrice)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.OrderItemOptions)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.OrderItemToppings)

                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.ProductPrice)

                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemOptions)

                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemToppings);
            return order;
        }

        public IQueryable<Order> GetOrdersByBranchIdInStore(Guid? storeId, Guid? branchId)
        {
            var orders = dbSet.Where(o => o.StoreId == storeId && o.BranchId == branchId);

            return orders;
        }

        public IQueryable<Order> GetOrdersNotCancleByShiftInStore(Guid? storeId, Guid? branchId)
        {
            var orders = dbSet.Where(o => o.StoreId == storeId && o.BranchId == branchId && o.StatusId != EnumOrderStatus.Canceled);

            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(Guid? storeId, Guid? branchId, Guid? orderId)
        {
            var order = await dbSet
                .Where(o => o.StoreId == storeId && o.BranchId == branchId && o.Id == orderId)
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(Guid? storeId, Guid? orderId)
        {
            var order = await dbSet
                .Where(o => o.StoreId == storeId && o.Id == orderId)
                .FirstOrDefaultAsync();

            return order;
        }

        /// <summary>
        /// This method is used to get the current order by order code.
        /// </summary>
        /// <param name="code">For example: 770</param>
        /// <returns></returns>
        public async Task<Order> GetOrderByCode(string code)
        {
            var order = await dbSet.FirstOrDefaultAsync(order => order.Code == code);

            return order;
        }

        public IQueryable<Order> CountOrderByStoreBranchId(Guid? storeId, Guid? branchId)
        {
            var orders = dbSet.Where(o => o.StoreId == storeId &&
            (o.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid || (o.OrderPaymentStatusId == EnumOrderPaymentStatus.Unpaid && o.StatusId != EnumOrderStatus.Draft)) &&
            (branchId.HasValue ? o.BranchId == branchId.Value : true));

            return orders;
        }

        /// <summary>
        /// This method is used to get the user's orders.
        /// </summary>
        /// <param name="customerId">For example: b3b849b4-2344-4755-ac53-03ce0df246ee</param>
        /// <returns></returns>
        public IQueryable<Order> GetOrderListByCustomerId(Guid? customerId)
        {
            var orders = _dbContext.Orders.Where(order => order.Customer.AccountId == customerId);
            return orders;
        }

        public async Task<Order> GetOrderDetailByIdAsync(Guid customerId, Guid orderId)
        {
            var order = await dbSet
                .Include(od => od.OrderDelivery)
                .Include(st => st.Store).ThenInclude(a => a.StoreBranches)
                .Include(st => st.Store).ThenInclude(a => a.Currency)
                .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Id == orderId);
            return order;
        }
    }
}

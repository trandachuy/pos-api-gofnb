using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerMembershipLevel> GetCustomerMembershipByCustomerIdAsync(Guid customerId, Guid storeId);

        Task<decimal> GetCustomerMembershipDiscountValueByCustomerIdAsync(decimal price, Guid customerId, Guid storeId);
    }
}

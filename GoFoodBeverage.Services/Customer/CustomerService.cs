using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services.Order
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerMembershipLevel> GetCustomerMembershipByCustomerIdAsync(Guid customerId, Guid storeId)
        {
            var customer = await _unitOfWork.Customers
                .GetAllCustomersInStore(storeId)
                .Include(x => x.CustomerPoint)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            if (customer == null) return null;

            var customerPoint = customer?.CustomerPoint;
            var customerMemberships = await _unitOfWork.CustomerMemberships.GetAllCustomerMembershipInStore(storeId).OrderBy(x => x.AccumulatedPoint).ToListAsync();

            /// Mock data for not found any data from table
            if (customerMemberships.Count == 0)
            {
                var defaultCustomerMembership = new CustomerMembershipLevel()
                {
                    Name = "Default membership",
                    MaximumDiscount = 0,
                    Discount = 0,
                    Description = "Default membership",
                    AccumulatedPoint = 0,
                };

                return defaultCustomerMembership;
            }

            var maxCustomerMembership = customerMemberships.Aggregate((currentMax, next) => currentMax == null || customerPoint.AccumulatedPoint >= next.AccumulatedPoint ? next : currentMax);
            return maxCustomerMembership;
        }

        public async Task<decimal> GetCustomerMembershipDiscountValueByCustomerIdAsync(decimal price, Guid customerId, Guid storeId)
        {
            var customerMembershipLevel = await GetCustomerMembershipByCustomerIdAsync(customerId, storeId);
            if (customerMembershipLevel != null)
            {
                decimal discountValue = price * customerMembershipLevel.Discount / 100;
                if (customerMembershipLevel.MaximumDiscount.HasValue)
                {
                    return discountValue > customerMembershipLevel.MaximumDiscount.Value ? customerMembershipLevel.MaximumDiscount.Value : discountValue;
                }

                return discountValue;
            }

            return 0;
        }

    }
}

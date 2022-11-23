using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICustomerMembershipRepository : IGenericRepository<CustomerMembershipLevel>
    {
        IQueryable<CustomerMembershipLevel> GetAllCustomerMembershipInStore(Guid? storeId);

        Task<CustomerMembershipLevel> GetCustomerMembershipDetailByIdAsync(Guid customerMembershipId, Guid? storeId);

        bool CheckCustomerMemberShipByAccumulatedPointInStore(int accumulatedPoint, Guid? storeId);

        bool CheckCustomerMemberShipByNameInStore(string name, Guid? storeId);
    }
}

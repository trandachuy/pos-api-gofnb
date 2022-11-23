using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        IQueryable<Customer> GetCustomerByKeySearchInStore(string keySearch, Guid? storeId);

        IQueryable<Customer> GetAllCustomersInStore(Guid storeId);

        IQueryable<Customer> GetCustomersbyAccumulatedPointInStore(int accumulatedPoint, Guid? storeId);

        /// <summary>
        /// Get customer data by execute sp 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        List<Customer> GetCustomerDataBySegmentInStore(Guid storeId, Guid segmentId);

        bool CheckCustomerByNameInStore(string name, Guid? storeId);

        bool CheckCustomerByPhoneInStore(string phone, Guid? storeId);

        bool CheckCustomerByEmailInStore(string email, Guid? storeId);

        Task<Customer> GetCustomerByIdInStore(Guid id, Guid? storeId);

        Task<Customer> GetCustomerByIdAsync(Guid id);
    }
}

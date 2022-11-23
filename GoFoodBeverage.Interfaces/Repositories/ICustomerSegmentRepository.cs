using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICustomerSegmentRepository : IGenericRepository<CustomerSegment>
    {
        IQueryable<CustomerSegment> GetAllCustomerSegmentsInStore(Guid storeId);

        Task<CustomerSegment> GetCustomerSegmentByNameInStoreAsync(string customerSegmentName, Guid storeId);

        Task<CustomerSegment> GetCustomerSegmentDetailByIdAsync(Guid customerSegmentId, Guid? storeId);

        Task<CustomerSegment> CheckExistCustomerSegmentNameInStoreAsync(Guid customerSegmentId, string customerSegmentName, Guid storeId);
    }
}

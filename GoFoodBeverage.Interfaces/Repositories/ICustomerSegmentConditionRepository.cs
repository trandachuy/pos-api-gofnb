using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface ICustomerSegmentConditionRepository : IGenericRepository<CustomerSegmentCondition>
    {
        IQueryable<CustomerSegmentCondition> GetAllConditionsByCustomerSegmentId(Guid customerSegmentId);
    }
}

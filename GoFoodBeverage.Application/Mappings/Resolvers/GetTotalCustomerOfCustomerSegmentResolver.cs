using AutoMapper;
using GoFoodBeverage.Domain.Entities;
using System.Linq;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class GetTotalCustomerOfCustomerSegmentResolver : IValueResolver<CustomerSegment, object, int>
    {
        public int Resolve(CustomerSegment source, object destination, int destMember, ResolutionContext context)
        {
            if (source.CustomerCustomerSegments == null)
                return 0;

            int totalCustomer = source.CustomerCustomerSegments.Count();
            return totalCustomer;
        }
    }
}

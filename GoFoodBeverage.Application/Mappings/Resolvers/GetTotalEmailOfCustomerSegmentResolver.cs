using AutoMapper;
using GoFoodBeverage.Domain.Entities;
using System.Linq;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class GetTotalEmailOfCustomerSegmentResolver : IValueResolver<CustomerSegment, object, int>
    {
        public int Resolve(CustomerSegment source, object destination, int destMember, ResolutionContext context)
        {
            if (source.CustomerCustomerSegments == null)
                return 0;

            int totalEmail = source.CustomerCustomerSegments.Where(a => !string.IsNullOrEmpty(a.Customer.Email)).Count();
            return totalEmail;
        }
    }
}

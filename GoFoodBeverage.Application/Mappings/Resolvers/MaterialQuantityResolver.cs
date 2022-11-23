using AutoMapper;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class MaterialQuantityResolver : IValueResolver<Material, object, int?>
    {
        public int? Resolve(Material source, object destination, int? destMember, ResolutionContext context)
        {
            int? sumQuantity = source.MaterialInventoryBranches.Sum(a => a.Quantity);

            return sumQuantity;
        }
    }
}

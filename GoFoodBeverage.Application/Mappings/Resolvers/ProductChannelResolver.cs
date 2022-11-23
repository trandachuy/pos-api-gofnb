using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class ProductChannelResolver : IValueResolver<Product, object, IEnumerable<ChannelModel>>
    {
        private readonly IMapper _mapper;

        public ProductChannelResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<ChannelModel> Resolve(Product source, object destination, IEnumerable<ChannelModel> destMember, ResolutionContext context)
        {
            var channels = source.ProductChannels.Select(p => p.Product);
            var response = _mapper.Map<IEnumerable<ChannelModel>>(channels);

            return response;
        }
    }
}

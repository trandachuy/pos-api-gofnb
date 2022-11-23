using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.POS.Models.Product;

namespace GoFoodBeverage.POS.Application.Mappings.Resolvers
{
    public class ProductOptionsResolver : IValueResolver<Product, object, List<ProductOptionDto>>
    {
        public List<ProductOptionDto> Resolve(Product source, object destination, List<ProductOptionDto> destMember, ResolutionContext context)
        {
            List<ProductOptionDto> result = new List<ProductOptionDto>();
            if (source.ProductOptions == null)
                return result;
            var optionFromProduct = source.ProductOptions
                .Select(a => a.Option)
                .SelectMany(a => a.OptionLevel)
                .Where(a => a.IsSetDefault == true)
                .ToList();

            foreach (var option in optionFromProduct)
            {
                result.Add(new ProductOptionDto()
                {
                    OptionId = option.OptionId,
                    OptionLevelId = option.Id,
                    OptionLevelName = option.Name,
                    OptionName = option.Option.Name
                });
            }

            return result;
        }
    }
}

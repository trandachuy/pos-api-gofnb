using System;
using AutoMapper;
using System.Linq;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Common;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class ProductStatusResolver : IValueResolver<Product, object, StatusModel>
    {
        public StatusModel Resolve(Product source, object destination, StatusModel destMember, ResolutionContext context)
        {
            var estatuses = Enum.GetValues(typeof(EnumStatus)).Cast<EnumStatus>();
            var status = estatuses.FirstOrDefault(s => (int)s == source.StatusId);
            return new StatusModel()
            {
                Id = (int)status,
                Name = status.GetDescription()
            };
        }
    }
}

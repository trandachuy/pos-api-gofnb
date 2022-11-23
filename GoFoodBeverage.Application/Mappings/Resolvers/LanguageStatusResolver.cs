using System;
using AutoMapper;
using System.Linq;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Common;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Models.Language.Dto;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class LanguageStatusResolver : IValueResolver<LanguageStoreDto, object, StatusModel>
    {
        public StatusModel Resolve(LanguageStoreDto source, object destination, StatusModel destMember, ResolutionContext context)
        {
            var estatuses = Enum.GetValues(typeof(EnumLanguageStore)).Cast<EnumLanguageStore>();
            var status = estatuses.FirstOrDefault(s => (int)s == Convert.ToInt32(source.IsPublish));
            return new StatusModel()
            {
                Id = (int)status,
                Name = status.GetDescription()
            };
        }
    }
}

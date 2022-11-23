using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Models.Package;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class PackageStatusResolver : IValueResolver<OrderPackage, PackageOrderModel, EnumPackageStatus>
    {
        private readonly IDateTimeService _dateTimeService;

        public PackageStatusResolver(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        public EnumPackageStatus Resolve(OrderPackage sources, PackageOrderModel destination, EnumPackageStatus destMember, ResolutionContext context)
        {
            EnumPackageStatus enumPackageStatus = EnumPackageStatus.Inactive;
            EnumOrderPackageStatus orderPackageStatus = HelperExtensions.ParseStringToEnum<EnumOrderPackageStatus>(sources.Status);
            if (orderPackageStatus == EnumOrderPackageStatus.APPROVED)
            {
                var utcNow = _dateTimeService.NowUtc;
                if(sources.ExpiredDate.HasValue && utcNow < sources.ExpiredDate)
                {
                    enumPackageStatus = EnumPackageStatus.Active;
                }
            }

            return enumPackageStatus;
        }
    }
}

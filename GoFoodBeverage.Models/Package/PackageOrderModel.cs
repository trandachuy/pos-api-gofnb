using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Package
{
    public class PackageOrderModel
    {
        public int Code { get; set; }

        public string Title { get; set; }

        public int PackageDurationByMonth { get; set; }

        public Guid PackageId { get; set; }

        public EnumOrderPaymentStatus EnumOrderPaymentStatus { get; set; }

        public string EnumOrderPaymentStatusName { get { return EnumOrderPaymentStatusExtensions.GetName(EnumOrderPaymentStatus); } }

        public DateTime? CreatedTime { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public PackageDto Package { get; set; }

        public class PackageDto
        {
            public string Name { get; set; }
        }

        public EnumPackageStatus EnumPackageStatusId { get; set; }

        public string EnumPackageStatus { get { return EnumPackageStatusExtensions.GetName(EnumPackageStatusId); } }
    }    
}

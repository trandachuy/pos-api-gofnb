using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderPackageStatus
    {
        PENDING = 0,

        APPROVED = 1,

        CANCELED = 2
    }

    public static class EnumOrderPackageStatusExtensions
    {
        public static string GetName(this EnumOrderPackageStatus enums) => enums switch
        {
            EnumOrderPackageStatus.PENDING => "PENDING",
            EnumOrderPackageStatus.APPROVED => "APPROVED",
            EnumOrderPackageStatus.CANCELED => "CANCELED",
            _ => string.Empty
        };
    }
}

using System;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPackage
    {
        POS = 1,

        WEB,

        APP
    }

    public static class EnumPackageExtensions
    {
        public static Guid ToGuid(this EnumPackage enums) => enums switch
        {
            EnumPackage.POS => new Guid("6c626154-5065-6361-6b61-676500000001"),
            EnumPackage.WEB => new Guid("6c626154-5065-6361-6b61-676500000002"),
            EnumPackage.APP => new Guid("6c626154-5065-6361-6b61-676500000003"),
            _ => new Guid("00000000-0000-0000-0000-000000000000")
        };
    }
}

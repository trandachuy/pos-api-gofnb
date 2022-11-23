using System;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPlatform
    {
        /// <summary>
        /// AdminWebsite
        /// </summary>
        AdminWebsite = 0,

        /// <summary>
        /// AdminMobileApp
        /// </summary>
        AdminMobileApp = 1,

        /// <summary>
        /// POSWebsite
        /// </summary>
        POSWebsite = 2,

        /// <summary>
        /// POSMobileApp
        /// </summary>
        POSMobileApp = 3,

        /// <summary>
        /// StoreWebsite
        /// </summary>
        StoreWebsite = 4,

        /// <summary>
        /// StoreMobileApp
        /// </summary>
        StoreMobileApp = 5,

        /// <summary>
        /// OrderWebsite
        /// </summary>
        OrderWebsite = 6,

        /// <summary>
        /// OrderWobileApp
        /// </summary>
        OrderWobileApp = 7,

        /// <summary>
        /// POS
        /// </summary>
        POS = 8,

        /// <summary>
        /// GoF&B App
        /// </summary>
        GoFnBApp = 9
    }

    public static class EnumPlatformExtensions
    {
        public static string ToString(this EnumPlatform enums) => enums switch
        {
            EnumPlatform.AdminWebsite => "AdminWebsite",
            EnumPlatform.AdminMobileApp => "AdminMobileApp",
            EnumPlatform.POSWebsite => "POSWebsite",
            EnumPlatform.POSMobileApp => "POSMobileApp",
            EnumPlatform.StoreWebsite => "StoreWebsite",
            EnumPlatform.StoreMobileApp => "StoreMobileApp",
            EnumPlatform.OrderWebsite => "OrderWebsite",
            EnumPlatform.OrderWobileApp => "OrderWobileApp",
            EnumPlatform.POS => "POS",
            EnumPlatform.GoFnBApp => "GoF&B App",
            _ => string.Empty
        };

        public static Guid ToGuid(this EnumPlatform enums) => enums switch
        {
            EnumPlatform.AdminWebsite => new Guid("6C626154-5065-616C-7466-6F7200000000"),
            EnumPlatform.AdminMobileApp => new Guid("6C626154-5065-616C-7466-6F7200000001"),
            EnumPlatform.POSWebsite => new Guid("6C626154-5065-616C-7466-6F7200000002"),
            EnumPlatform.POSMobileApp => new Guid("6C626154-5065-616C-7466-6F7200000003"),
            EnumPlatform.StoreWebsite => new Guid("6C626154-5065-616C-7466-6F7200000004"),
            EnumPlatform.StoreMobileApp => new Guid("6C626154-5065-616C-7466-6F7200000005"),
            EnumPlatform.OrderWebsite => new Guid("6C626154-5065-616C-7466-6F7200000006"),
            EnumPlatform.OrderWobileApp => new Guid("6C626154-5065-616C-7466-6F7200000007"),
            EnumPlatform.POS => new Guid("6C626154-5065-616C-7466-6F7200000008"),
            EnumPlatform.GoFnBApp => new Guid("6C626154-5065-616C-7466-6F7200000009"),
            _ => new Guid("00000000-0000-0000-0000-000000000000")
        };
    }

}

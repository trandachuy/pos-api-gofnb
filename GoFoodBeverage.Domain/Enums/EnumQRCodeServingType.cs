using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumQRCodeServingType
    {
        /// <summary>
        /// Instore
        /// </summary>
        Instore = 0,

        /// <summary>
        /// Delivery
        /// </summary>
        Delivery = 1,

        /// <summary>
        /// Take Away
        /// </summary>
        TakeAway = 2,

        /// <summary>
        /// Online
        /// </summary>
        Online = 3
    }

    public static class EnumQRCodeServingTypeExtensions
    {
        public static string GetName(this EnumQRCodeServingType enums) => enums switch
        {
            EnumQRCodeServingType.Instore => "createQrCode.servingType.instore",
            EnumQRCodeServingType.Online => "createQrCode.servingType.online",
            _ => string.Empty
        };
    }
}

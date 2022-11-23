
namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumDeliveryMethod
    {
        /// <summary>
        /// Self Delivery
        /// </summary>
        SelfDelivery = 1,

        /// <summary>
        /// AhaMove
        /// </summary>
        AhaMove = 2,

        COD = 3,
    }

    public static class EnumDeliveryMethodExtensions
    {
        public static string GetName(this EnumDeliveryMethod enums) => enums switch
        {
            EnumDeliveryMethod.SelfDelivery => "Self Delivery",
            EnumDeliveryMethod.AhaMove => "AhaMove",
            EnumDeliveryMethod.COD => "COD",
            _ => string.Empty
        };
    }
}

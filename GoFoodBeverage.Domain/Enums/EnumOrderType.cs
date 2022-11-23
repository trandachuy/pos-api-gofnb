namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderType
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

    public class EnumOrderTypeSymbol
    {
        public const string InStore = "I";

        public const string Delivery = "D";

        public const string TakeAway = "T";

        public const string Online = "O";
    }

    public static class EnumOrderTypeExtensions
    {
        public static string GetFirstCharacter(this EnumOrderType enums) => enums switch
        {
            EnumOrderType.Instore => EnumOrderTypeSymbol.InStore,
            EnumOrderType.Delivery => EnumOrderTypeSymbol.Delivery,
            EnumOrderType.TakeAway => EnumOrderTypeSymbol.TakeAway,
            EnumOrderType.Online => EnumOrderTypeSymbol.Online,
            _ => string.Empty
        };

        public static string GetName(this EnumOrderType enums) => enums switch
        {
            EnumOrderType.Instore => "In Store",
            EnumOrderType.Delivery => "Delivery",
            EnumOrderType.TakeAway => "Take Away",
            EnumOrderType.Online => "Online",
            _ => string.Empty
        };
    }
}

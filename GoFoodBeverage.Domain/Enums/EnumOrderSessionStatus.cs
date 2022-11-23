using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderSessionStatus
    {
        /// <summary>
        /// OrderSession new
        /// </summary>
        New = 1,

        /// <summary>
        /// OrderSession completed
        /// </summary>
        Completed = 2,
    }

    public static class EnumOrderSessionStatusExtensions
    {
        public static string GetName(this EnumOrderSessionStatus enums) => enums switch
        {
            EnumOrderSessionStatus.New => "New",
            EnumOrderSessionStatus.Completed => "Completed",
            _ => string.Empty
        };
    }
}

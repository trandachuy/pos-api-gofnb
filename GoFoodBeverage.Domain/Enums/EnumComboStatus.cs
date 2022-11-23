
namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumComboStatus
    {
        /// <summary>
        /// Scheduled
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// Active
        /// </summary>
        Active = 2,

        /// <summary>
        /// Finished
        /// </summary>
        Finished = 3,
    }

    public static class EnumComboStatusExtensions
    {
        public static string getName(this EnumComboStatus enums) => enums switch
        {
            EnumComboStatus.Scheduled => "Scheduled",
            EnumComboStatus.Active => "Active",
            EnumComboStatus.Finished => "Finished",
            _ => string.Empty
        };
    }
}

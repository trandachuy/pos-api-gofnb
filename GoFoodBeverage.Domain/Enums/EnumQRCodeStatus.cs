namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumQRCodeStatus
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

    public static class EnumQRCodeStatusExtensions
    {
        public static string GetName(this EnumQRCodeStatus enums) => enums switch
        {
            EnumQRCodeStatus.Scheduled => "promotion.status.scheduled",
            EnumQRCodeStatus.Active => "promotion.status.active",
            EnumQRCodeStatus.Finished => "promotion.status.finished",
            _ => string.Empty
        };
    }
}

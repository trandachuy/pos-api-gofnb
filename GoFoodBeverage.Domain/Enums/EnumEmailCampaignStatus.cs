namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumEmailCampaignStatus
    {
        /// <summary>
        /// Scheduled
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// Finished
        /// </summary>
        Sent = 2,
    }

    public static class EnumEmailCampaignStatusExtensions
    {
        public static string GetName(this EnumEmailCampaignStatus enums) => enums switch
        {
            EnumEmailCampaignStatus.Scheduled => "marketing.emailCampaign.status.scheduled",
            EnumEmailCampaignStatus.Sent => "marketing.emailCampaign.status.sent",
            _ => string.Empty
        };
    }
}

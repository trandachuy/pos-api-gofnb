namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumEmailCampaignType
    {
        /// <summary>
        /// Send to email address
        /// </summary>
        SendToEmailAddress = 0,

        /// <summary>
        /// Send to customer group
        /// </summary>
        SendToCustomerGroup = 1
    }

    public static class EnumEmailCampaignTypeExtensions
    {
        public static string GetName(this EnumEmailCampaignType enums) => enums switch
        {
            EnumEmailCampaignType.SendToEmailAddress => "marketing.emailCampaign.sendToEmailAddress",
            EnumEmailCampaignType.SendToCustomerGroup => "marketing.emailCampaign.sendToCustomerGroup",
            _ => string.Empty
        };
    }
}

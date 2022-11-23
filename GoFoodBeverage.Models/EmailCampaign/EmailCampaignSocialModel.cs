using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.EmailCampaign
{
    public class EmailCampaignSocialModel
    {
        public EnumEmailCampaignSocial EnumEmailCampaignSocialId { get; set; }

        public bool IsActive { get; set; }

        public string Url { get; set; }
    }
}

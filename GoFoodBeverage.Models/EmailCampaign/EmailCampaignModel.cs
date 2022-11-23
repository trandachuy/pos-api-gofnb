using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Models
{
    public class EmailCampaignModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime SendingTime { get; set; }
         
        public EnumEmailCampaignStatus Status
        {
            get
            {
                var result = DateTime.UtcNow.CompareTo(SendingTime);
                return result < 0 ? EnumEmailCampaignStatus.Scheduled : EnumEmailCampaignStatus.Sent;
            }
        }
    }
}

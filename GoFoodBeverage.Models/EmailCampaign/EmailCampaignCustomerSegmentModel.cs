using System;

namespace GoFoodBeverage.Models.EmailCampaign
{
    public class EmailCampaignCustomerSegmentModel
    {
        public Guid? StoreId { get; set; }

        public Guid EmailCampaignId { get; set; }

        public Guid CustomerSegmentId { get; set; }
    }
}

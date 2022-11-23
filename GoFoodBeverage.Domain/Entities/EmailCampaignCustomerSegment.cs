using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(EmailCampaignCustomerSegment))]
    public class EmailCampaignCustomerSegment : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid EmailCampaignId { get; set; }

        public Guid CustomerSegmentId { get; set; }

        public virtual EmailCampaign EmailCampaign { get; set; }

        public virtual CustomerSegment CustomerSegment { get; set; }
    }
}

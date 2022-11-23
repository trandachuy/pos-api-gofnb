using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(EmailCampaignDetail))]
    public class EmailCampaignDetail : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid EmailCampaignId { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string ButtonName { get; set; }

        [MaxLength(100)]
        public string ButtonLink { get; set; }

        public string Thumbnail { get; set; }

        public int Position { get; set; }

        public virtual EmailCampaign EmailCampaign { get; set; }
    }
}

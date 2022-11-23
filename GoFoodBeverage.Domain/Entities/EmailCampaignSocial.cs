using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(EmailCampaignSocial))]
    public class EmailCampaignSocial : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid EmailCampaignId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsActive { get; set; }

        public virtual EmailCampaign EmailCampaign { get; set; }
    }
}

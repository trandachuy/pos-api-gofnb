using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(EmailCampaignSendingTransaction))]
    public class EmailCampaignSendingTransaction : BaseEntity
    {
        public Guid EmailCampaignId { get; set; }

        public string CustomerEmail { get; set; }

        public EnumEmailCampaignSendingStatus Status { get; set; }

        #region Navigations

        public virtual EmailCampaign EmailCampaign { get; set; }

        #endregion Navigations
    }
}
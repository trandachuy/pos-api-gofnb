using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(EmailCampaign))]
    public class EmailCampaign : BaseEntity
    {
        public Guid StoreId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Description("Default is current date & (current time + 15 minutes)")]
        public DateTime SendingTime { get; set; }

        [MaxLength(255)]
        [Required]
        public string EmailSubject { get; set; }

        [Description("Option: 'Send to email address', 'Send to customer group'")]
        public EnumEmailCampaignType EmailCampaignType { get; set; }

        [MaxLength(100)]
        public string EmailAddress { get; set; }

        [MaxLength(10)]
        [Column(TypeName = "varchar")]
        public string PrimaryColor { get; set; }

        [MaxLength(10)]
        [Column(TypeName = "varchar")]
        public string SecondaryColor { get; set; }

        public string LogoUrl { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string FooterContent { get; set; }

        [Column(TypeName = "ntext")]
        public string Template { get; set; }

        public virtual ICollection<EmailCampaignCustomerSegment> EmailCampaignCustomerSegments { get; set; }

        public virtual ICollection<EmailCampaignDetail> EmailCampaignDetails { get; set; }

        public virtual ICollection<EmailCampaignSocial> EmailCampaignSocials { get; set; }

        public virtual ICollection<EmailCampaignSendingTransaction> EmailCampaignSendingTransactions { get; set; }
    }
}

using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreBanner))]
    public class StoreBanner : BaseEntity
    {
        public Guid StoreId { get; set; }

        [Description("Set banner type for POS 2nd screen")]
        public EnumBannerType Type { get; set; } = EnumBannerType.FullScreen;

        [Description("Banner image URL")]
        public string Thumbnail { get; set; }

        public virtual Store Store { get; set; }
    }
}
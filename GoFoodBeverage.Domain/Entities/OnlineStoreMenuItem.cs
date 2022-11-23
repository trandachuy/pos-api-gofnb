using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OnlineStoreMenuItem))]
    public class OnlineStoreMenuItem : BaseEntity
    {
        public Guid MenuId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        public EnumHyperlinkMenu HyperlinkOption { get; set; }

        [MaxLength(2000)]
        public string Url { get; set; }

        [Description("If HyperlinkOption='SubMenu' get Id from OnlineStoreMenu")]
        public Guid? SubMenuId { get; set; }

        public int? Position { get; set; }

        [JsonIgnore]
        public virtual OnlineStoreMenu OnlineStoreMenu { get; set; }
    }
}

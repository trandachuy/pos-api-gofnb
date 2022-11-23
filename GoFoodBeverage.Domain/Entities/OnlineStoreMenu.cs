using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OnlineStoreMenu))]
    public class OnlineStoreMenu : BaseEntity
    {
        public Guid StoreId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [Description("Option: 'Level 1', 'Level 2'")]
        public EnumLevelMenu Level { get; set; }

        public bool IsDefault { get; set; }

        public virtual ICollection<OnlineStoreMenuItem> OnlineStoreMenuItems { get; set; }
    }
}

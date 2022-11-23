using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GoFoodBeverage.Models.OnlineStoreMenus
{
    public class OnlineStoreMenuModel
    {

        public Guid StoreId { get; set; }

        public string Name { get; set; }

        [Description("Option: 'Level 1', 'Level 2'")]
        public EnumLevelMenu Level { get; set; }

        public bool IsDefault { get; set; }

        public DateTime? CreatedTime { get; set; }

        public  IList<OnlineStoreMenuItemModel> OnlineStoreMenuItems { get; set; }
    }
}

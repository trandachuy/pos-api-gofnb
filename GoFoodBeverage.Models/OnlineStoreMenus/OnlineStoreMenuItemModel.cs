using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GoFoodBeverage.Models.OnlineStoreMenus
{
    public class OnlineStoreMenuItemModel
    {
        public Guid MenuId { get; set; }

        public string Name { get; set; }

        public EnumHyperlinkMenu HyperlinkOption { get; set; }

        public string Url { get; set; }

        [Description("If HyperlinkOption='SubMenu' get Id from OnlineStoreMenu")]
        public Guid? SubMenuId { get; set; }

        public int? Position { get; set; }
    }
}

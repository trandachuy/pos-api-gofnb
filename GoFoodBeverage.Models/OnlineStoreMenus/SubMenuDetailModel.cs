using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.OnlineStoreMenus
{
    public class SubMenuDetailModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public EnumLevelMenu? Level { get; set; }

        public bool? IsDefault { get; set; }

        public List<MenuItemModel> OnlineStoreMenuItems { get; set; }

        public class MenuItemModel
        {
            public Guid? Id { get; set; }

            public Guid? MenuId { get; set; }

            public string Name { get; set; }

            public EnumHyperlinkMenu? HyperlinkOption { get; set; }

            public string Url { get; set; }

            public Guid? SubMenuId { get; set; }

            public int? Position { get; set; }
        }
    }
}

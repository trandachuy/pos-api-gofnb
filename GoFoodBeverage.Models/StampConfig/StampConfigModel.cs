using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Models.StampConfig
{
    public class StampConfigModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public EnumStampType StampType { get; set; }

        public bool IsShowLogo { get; set; }

        public bool IsShowTime { get; set; }

        public bool IsShowNumberOfItem { get; set; }

        public bool IsShowNote { get; set; }
    }
}

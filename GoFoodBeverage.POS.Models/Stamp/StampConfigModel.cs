using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.POS.Models.Stamp
{
    public class StampConfigModel
    {
        public EnumStampType StampType { get; set; }

        public bool IsShowLogo { get; set; }

        public bool IsShowTime { get; set; }

        public bool IsShowNumberOfItem { get; set; }

        public bool IsShowNote { get; set; }
    }
}

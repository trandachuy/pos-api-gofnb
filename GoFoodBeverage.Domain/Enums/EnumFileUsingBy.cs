using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumFileUsingBy
    {
        [Description("Using by slider. File type is media")]
        Slider = 0,

        [Description("Order")]
        Order = 99999,
    }
}

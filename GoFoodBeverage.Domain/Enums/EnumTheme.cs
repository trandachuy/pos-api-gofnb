
using System;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumTheme
    {
        Default = 0,

        Ciao = 1,

        Frappe = 2,
    }

    public static class EnumThemeExtensions
    {

        public static string GetName(this EnumTheme enums) => enums switch
        {
            EnumTheme.Default => "The Default",
            EnumTheme.Ciao => "Ciao",
            EnumTheme.Frappe => "Frappé",   
            _ => string.Empty
        };

        public static string GetImage(this EnumTheme enums) => enums switch
        {
            EnumTheme.Default => "assets/images/default.png",
            EnumTheme.Ciao => "assets/images/ciao.png",
            EnumTheme.Frappe => "assets/images/frappe.png",
            _ => string.Empty
        };

        public static string GetTags(this EnumTheme enums) => enums switch
        {
            EnumTheme.Default => "Age verifier,Infinite scroll,Sticky header",
            EnumTheme.Ciao => "Quick view,Mega menu, Sticky header",
            EnumTheme.Frappe => "EU translations (EN FR IT DE ES),Store locator,Quick view,Book Table,Qick buy",
            _ => string.Empty
        };

        public static string GetDescription(this EnumTheme enums) => enums switch
        {
            EnumTheme.Default => "Description of Default Theme",
            EnumTheme.Ciao => "Description of Ciao Theme",
            EnumTheme.Frappe => "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
            _ => string.Empty
        };

        public static Guid ToGuid(this EnumTheme enums) => enums switch
        {
            EnumTheme.Default => new Guid("921016fe-d34e-4192-beb8-15d775d0ee5b"),
            EnumTheme.Ciao => new Guid("46565f44-c3e2-449d-8d58-3850a95ffba7"),
            EnumTheme.Frappe => new Guid("526cb94e-3973-4fba-b4f4-80b53a7db652"),
            _ => Guid.Empty
        };
    }
}


namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumLevelMenu
    {
        /// <summary>
        /// Level 1
        /// </summary>
        Level1 = 1,

        /// <summary>
        /// Level 2
        /// </summary>
        Level2 = 2,
    }

    public static class EnumLevelMenuExtensions
    {
        public static string GetName(this EnumLevelMenu enums) => enums switch
        {
            EnumLevelMenu.Level1 => "menuOnlineStore.level1",
            EnumLevelMenu.Level2 => "menuOnlineStore.level2",
            _ => string.Empty
        };
    }
}

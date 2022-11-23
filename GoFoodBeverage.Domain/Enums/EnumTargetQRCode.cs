namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumTargetQRCode
    {
        /// <summary>
        /// ShopMenu
        /// </summary>
        ShopMenu = 0,

        /// <summary>
        /// AddProductToCart
        /// </summary>
        AddProductToCart = 1
    }

    public static class EnumTargetQRCodeExtensions
    {
        public static string GetName(this EnumTargetQRCode enums) => enums switch
        {
            EnumTargetQRCode.ShopMenu => "marketing.qrCode.shopMenu",
            EnumTargetQRCode.AddProductToCart => "marketing.qrCode.addProductToCart",
            _ => string.Empty
        };
    }
}

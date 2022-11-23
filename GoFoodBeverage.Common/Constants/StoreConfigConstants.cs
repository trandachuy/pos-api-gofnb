namespace GoFoodBeverage.Common.Constants
{
    public static class StoreConfigConstants
    {
        public const string PURCHASE_ORDER_CODE = "PURCHASE_ORDER_CODE";

        public const string ORDER_CODE = "ORDER_CODE";

        public const string PRODUCT_CATEGORY_CODE = "PRODUCT_CATEGORY_CODE";

        public const string OPTION_CODE = "OPTION_CODE";

        public const string TOPPING_CODE = "TOPPING_CODE";

        public const string MATERIAL_CODE = "MATERIAL_CODE";
    }

    public static class StoreConfigExtension
    {
        public static string GetCode(this int number, string type)
        {
            string code = string.Empty;
            switch(type)
            {
                case StoreConfigConstants.PURCHASE_ORDER_CODE:
                    code = $"{DefaultConstants.PURCHASE_ORDER_CODE_PREFIX}-{number:0000}";
                    break;

                case StoreConfigConstants.PRODUCT_CATEGORY_CODE:
                    code = $"{DefaultConstants.PRODUCT_CATEGORY_CODE_PREFIX}{number:0000}";
                    break;

                case StoreConfigConstants.OPTION_CODE:
                    code = $"{DefaultConstants.OPTION_CODE_PREFIX}{number:0000}";
                    break;

                case StoreConfigConstants.TOPPING_CODE:
                    code = $"{DefaultConstants.TOPPING_CODE_PREFIX}{number:0000}";
                    break;

                case StoreConfigConstants.MATERIAL_CODE:
                    code = $"{DefaultConstants.MATERIAL_CODE_PREFIX}{number:0000}";
                    break;
            }

            return code;
        }
    }
}

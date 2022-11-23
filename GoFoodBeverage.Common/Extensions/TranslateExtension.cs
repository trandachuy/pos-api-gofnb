
namespace GoFoodBeverage.Common.Extensions
{
    public static class TranslateExtension
    {
        /// <summary>
        /// Translate status name by language code. Default languageCode is en.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="languageCode"></param>
        /// <returns>true: "Active", false: "Inactive"</returns>
        public static string TranslateStatus(this bool status, string languageCode = "en")
        {
            return languageCode switch
            {
                "en" => status ? "Active" : "Inactive",
                "vi" => status ? "Kích hoạt" : "Tắt kích hoạt",
                _ => status.ToString(),
            };
        }
    }
}

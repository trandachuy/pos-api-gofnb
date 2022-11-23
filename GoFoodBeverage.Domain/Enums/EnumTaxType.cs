using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumTaxType
    {
        /// <summary>
        /// Selling Tax
        /// </summary>
        SellingTax = 0,

        /// <summary>
        /// Imported Tax
        /// </summary>
        ImportedTax = 1,
    }

    public static class EnumTaxTypeExtensions
    {
        public static string GetName(this EnumTaxType enums) => enums switch
        {
            EnumTaxType.SellingTax => "feeAndTax.tax.sellingTax",
            EnumTaxType.ImportedTax => "feeAndTax.tax.importTax",
            _ => string.Empty
        };
    }
}

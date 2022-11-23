using System;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductOptionDto
    {
        public Guid? OptionId { get; set; }

        public Guid? OptionLevelId { get; set; }

        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }

        public bool IsSetDefault { get; set; }
    }
}

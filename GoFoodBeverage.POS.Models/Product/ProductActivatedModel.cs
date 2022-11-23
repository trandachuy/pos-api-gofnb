using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Product
{
    /// <summary>
    /// This class response and display on POS platform
    /// </summary>
    public class ProductActivatedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsHasPromotion { get; set; } = false;

        public bool IsPromotionProductCategory { get; set; } = false;

        public bool IsDiscountPercent { get; set; } = false;

        public decimal? DiscountValue { get; set; }

        public decimal DiscountPrice { get; set; } = 0;

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public bool IsTopping { get; set; }

        public virtual UnitModel Unit { get; set; }

        public IEnumerable<ProductPriceModel> ProductPrices { get; set; }

        public IEnumerable<ProductOptionDto> ProductOptions { get; set; }

        public class ProductOptionDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public IEnumerable<OptionLevelDto> OptionLevels { get; set; }

            public class OptionLevelDto
            {
                public Guid Id { get; set; }

                public string Name { get; set; }

                public bool? IsSetDefault { get; set; }

                public Guid OptionId { get; set; }
            }
        }
    }
}
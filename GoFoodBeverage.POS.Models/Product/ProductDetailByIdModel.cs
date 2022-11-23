using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductDetailByIdModel
    {
        public Guid Id { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public string Name { get; set; }

        public bool IsHasPromotion { get; set; } = false;

        public bool IsPromotionProductCategory { get; set; } = false;

        public bool IsDiscountPercent { get; set; } = false;

        public decimal? DiscountValue { get; set; }

        public decimal DiscountPrice { get; set; } = 0;

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public List<ProductPriceDto> ProductPrices { get; set; }

        public class ProductPriceDto
        {
            public Guid Id { get; set; }

            public bool IsApplyPromotion { get; set; } = false;

            public string PriceName { get; set; }

            public decimal PriceValue { get; set; }

            public decimal OriginalPrice { get; set; }

            public DateTime? CreatedTime { get; set; }
        }

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

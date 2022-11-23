using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductPrice))]
    public class ProductPrice : BaseEntity
    {
        public Guid ProductId { get; set; }

        [MaxLength(100)]
        public string PriceName { get; set; }

        public decimal PriceValue { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<ProductPriceMaterial> ProductPriceMaterials { get; set; }

        public virtual ICollection<ComboProductPrice> ComboProductPrices { get; set; }

        public virtual ICollection<ComboPricingProductPrice> ComboPricingProducts { get; set; }
    }
}

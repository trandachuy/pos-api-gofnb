using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboPricing))]
    public class ComboPricing: BaseEntity
    {
        public Guid ComboId { get; set; }

        public string ComboName { get; set; }

        public decimal? OriginalPrice { get; set; }

        public decimal? SellingPrice { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Combo Combo { get; set; }

        public virtual ICollection<ComboPricingProductPrice> ComboPricingProducts { get; set; }
    }
}

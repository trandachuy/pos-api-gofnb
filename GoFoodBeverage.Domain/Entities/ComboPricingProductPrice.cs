using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboPricingProductPrice))]
    public class ComboPricingProductPrice: BaseEntity
    {
        public Guid? ComboPricingId { get; set; }

        public Guid? ProductPriceId { get; set; }

        /// <summary>
        /// Selling price on each combo
        /// </summary>
        public decimal? SellingPrice { get; set; }

        public Guid? StoreId { get; set; }

        public virtual ComboPricing ComboPricing { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }
    }
}

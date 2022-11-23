using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboProductGroup))]
    public class ComboProductGroup : BaseEntity
    {
        public Guid ComboId { get; set; }

        public Guid ProductCategoryId { get; set; }

        public int Quantity { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Combo Combo { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        /// <summary>
        /// List product prices
        /// </summary>
        public virtual ICollection<ComboProductGroupProductPrice> ComboProductGroupProductPrices { get; set; }
    }
}

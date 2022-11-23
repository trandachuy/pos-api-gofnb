using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboProductPrice))]
    public class ComboProductPrice : BaseAuditEntity
    {
        [Key]
        public Guid ComboId { get; set; }

        [Key]
        public Guid ProductPriceId { get; set; }

        public decimal PriceValue { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Combo Combo { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }
    }
}

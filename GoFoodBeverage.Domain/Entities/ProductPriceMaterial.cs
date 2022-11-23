using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductPriceMaterial))]
    public class ProductPriceMaterial : BaseAuditEntity
    {
        [Key]
        public Guid ProductPriceId { get; set; }

        [Key]
        public Guid MaterialId { get; set; }

        public int Quantity { get; set; }

        // public decimal Cost { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Material Material { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }
    }
}

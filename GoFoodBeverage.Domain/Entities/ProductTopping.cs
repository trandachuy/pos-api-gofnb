using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductTopping))]
    public class ProductTopping : BaseAuditEntity
    {
        /// <summary>
        /// This column stores the id of the product
        /// </summary>
        [Key]
        public Guid ProductId { get; set; }

        /// <summary>
        /// This column stores the id of the product with IsTopping = true
        /// </summary>
        [Key]
        public Guid ToppingId { get; set; }

        public Guid? StoreId { get; set; }
    }
}

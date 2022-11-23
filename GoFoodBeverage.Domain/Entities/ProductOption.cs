using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductOption))]
    public class ProductOption : BaseAuditEntity
    {
        [Key]
        public Guid ProductId { get; set; }

        [Key]
        public Guid OptionId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Option Option { get; set; }
    }
}

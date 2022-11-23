using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreTheme))]
    public class StoreTheme : BaseAuditEntity
    {
        [Key]
        public Guid StoreId { get; set; }

        [Key]
        public Guid ThemeId { get; set; }

        public virtual Store Store { get; set; }

        public virtual Theme Theme { get; set; }
    }
}

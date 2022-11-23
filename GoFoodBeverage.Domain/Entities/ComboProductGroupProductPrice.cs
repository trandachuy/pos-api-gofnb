using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboProductGroupProductPrice))]
    public class ComboProductGroupProductPrice : BaseEntity
    {
        public Guid ComboProductGroupId { get; set; }

        public Guid ProductPriceId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual ComboProductGroup ComboProductGroup { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }
    }
}

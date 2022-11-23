using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductChannel))]
    public class ProductChannel : BaseEntity
    {
        public Guid ProductId { get; set; }

        public Guid ChannelId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Channel Channel { get; set; }
    }
}

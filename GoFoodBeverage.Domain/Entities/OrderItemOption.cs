using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderItemOption))]
    public class OrderItemOption : BaseEntity
    {
        public Guid? OptionId { get; set; }

        public Guid? OptionLevelId { get; set; }

        public Guid OrderItemId { get; set; }

        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }

        public Guid? StoreId { get; set; }
    }
}

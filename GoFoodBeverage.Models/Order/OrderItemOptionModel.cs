using System;

namespace GoFoodBeverage.Models.Order
{
    public class OrderItemOptionModel
    {
        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }

        public Guid Id { get; set; }

        public Guid OptionLevelId { get; set; }
    }
}

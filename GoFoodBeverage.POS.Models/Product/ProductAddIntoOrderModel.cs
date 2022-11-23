using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductAddIntoOrderModel
    {
        public Guid? ProductId { get; set; }

        public int Quantity { get; set; }

        [Description("This field use checks when the increased quantity of product is a combo")]
        public Guid? ProductPriceId { get; set; }

        public List<OptionInformationOfProduct> OptionInformationOfProducts { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? OrderItemId { get; set; }
    }

    public class OptionInformationOfProduct
    {
        public Guid? OptionId { get; set; }

        public Guid? OptionLevelId { get; set; }

        public int Quantity { get; set; }
    }
}

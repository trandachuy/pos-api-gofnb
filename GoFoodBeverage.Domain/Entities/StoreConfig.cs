using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreConfig))]
    public class StoreConfig : BaseEntity
    {
        [Description("The configure related to store")]
        public Guid? StoreId { get; set; }

        [Description("Current maximum code of the purchase order belongs to store. The initial default is 1.")]
        public int CurrentMaxPurchaseOrderCode { get; set; } = 1;

        [Description("Current maximum code of the order belongs to store. The initial default is 1.")]
        public int CurrentMaxOrderCode { get; set; } = 1;

        [Description("Current maximum code of the product category belongs to store. The initial default is 1.")]
        [DefaultValue(1)]
        public int CurrentMaxProductCategoryCode { get; set; } = 1;

        [Description("Current maximum code of the option belongs to store. The initial default is 1.")]
        public int CurrentMaxOptionCode { get; set; } = 1;

        [Description("Current maximum code of the topping belongs to store. The initial default is 1.")]
        public int CurrentMaxToppingCode { get; set; } = 1;

        [Description("Current maximum code of the material belongs to store. The initial default is 1.")]
        public int CurrentMaxMaterialCode { get; set; } = 1;
    }
}

using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.Models.BarcodeConfig
{
    public class BarcodeConfigModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public EnumStampType StampType { get; set; }

        public EnumBarcodeType BarcodeType { get; set; }

        public bool IsShowName { get; set; }

        public bool IsShowPrice { get; set; }

        public bool IsShowCode { get; set; }
    }
}

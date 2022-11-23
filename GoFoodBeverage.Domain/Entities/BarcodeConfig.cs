using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(BarcodeConfig))]
    public class BarcodeConfig : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public EnumStampType StampType { get; set; }

        public EnumBarcodeType BarcodeType { get; set; }

        public bool IsShowName { get; set; }

        public bool IsShowPrice { get; set; }

        public bool IsShowCode { get; set; }


        public virtual Store Store { get; set; }
    }
}

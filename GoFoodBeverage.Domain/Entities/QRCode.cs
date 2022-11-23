using System;
using System.ComponentModel;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(QRCode))]
    public class QRCode : BaseEntity
    {
        public Guid StoreId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public Guid StoreBranchId { get; set; }

        [Description("EnumOrderType get 2 values: Instore(0) and Online(3)")]
        public EnumOrderType ServiceTypeId { get; set; }

        public Guid? AreaTableId { get; set; }

        [Description("The field has value when cloned from another qr code.")]
        public Guid? ClonedByQrCodeId { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Description("EnumTargetQRCode have 2 values: ShopMenu(0) and AddProductToCart(1)")]
        public EnumTargetQRCode TargetId { get; set; }

        [Description("If the value is true, have percent discount")]
        public bool IsPercentDiscount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PercentNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumDiscountAmount { get; set; }

        public bool IsStopped { get; set; }

        [JsonIgnore]
        public virtual QRCode ClonedByQrCode { get; set; }

        public virtual ICollection<QRCodeProduct> QRCodeProducts { get; set; }
    }
}

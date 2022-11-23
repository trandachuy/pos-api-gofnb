using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PurchaseOrderMaterial))]
    public class PurchaseOrderMaterial
    {
        [Key]
        public Guid PurchaseOrderId { get; set; }

        [Key]
        public Guid MaterialId { get; set; }

        public Guid? UnitId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }

        public Guid? LastSavedUser { get; set; }

        public DateTime? LastSavedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public DateTime? CreatedTime { get; set; }

        public Guid? StoreId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public virtual Material Material { get; set; }

        public virtual Unit Unit { get; set; }
    }
}

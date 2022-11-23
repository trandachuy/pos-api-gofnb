using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(QRCodeProduct))]
    public class QRCodeProduct : BaseEntity
    {
        public Guid QRCodeId { get; set; }

        public Guid ProductId { get; set; }

        public int ProductQuantity { get; set; }

        [JsonIgnore]
        public virtual QRCode QRCode { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}

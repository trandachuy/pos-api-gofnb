using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Unit))]
    public class Unit : BaseEntity
    {
        public Guid StoreId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public int Position { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Store Store { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Product> Products { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<PurchaseOrderMaterial> PurchaseOrderMaterials { get; set; }
        
        public virtual ICollection<UnitConversion> UnitConversions { get; set; }
    }
}

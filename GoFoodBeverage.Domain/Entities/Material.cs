using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Material))]
    public class Material : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? MaterialCategoryId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public string Sku { get; set; }

        public int? MinQuantity { get; set; }

        public int? Quantity { get; set; }

        public decimal? CostPerUnit { get; set; } // the number will be calculate on purchase order

        public bool? IsActive { get; set; }

        public bool HasExpiryDate { get; set; }

        public string Thumbnail { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Store Store { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Unit Unit { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual MaterialCategory MaterialCategory { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<PurchaseOrderMaterial> PurchaseOrderMaterials { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<MaterialInventoryBranch> MaterialInventoryBranches { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ProductPriceMaterial> ProductPriceMaterials { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<MaterialInventoryChecking> MaterialInventoryCheckings { get; set; }
    }
}

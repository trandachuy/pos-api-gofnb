using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Product))]
    public class Product : BaseEntity
    {
        public Guid StoreId { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? TaxId { get; set; }

        public int StatusId { get; set; } // mapping to EStatus enum

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public bool IsTopping { get; set; }

        /// <summary>
        /// Column for soft delete
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        public virtual Store Store { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual Tax Tax { get; set; }

        public virtual ICollection<ProductProductCategory> ProductProductCategories { get; set; }

        public virtual ICollection<ProductPrice> ProductPrices { get; set; }

        public virtual ICollection<ProductChannel> ProductChannels { get; set; }

        public virtual ICollection<ProductOption> ProductOptions { get; set; }

        public virtual ICollection<PromotionProduct> PromotionProducts { get; set; }

        public virtual ICollection<ComboProductGroupProductPrice> ProductGroups { get; set; }

        public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; }
    }
}

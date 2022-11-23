using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductCategory))]
    public class ProductCategory : BaseEntity
    {
        public Guid? StoreId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public int Priority { get; set; }

        public bool IsDisplayAllBranches { get; set; } = true;

        [MaxLength(15)]
        public string Code { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<ProductProductCategory> ProductProductCategories { get; set; }

        public virtual ICollection<StoreBranchProductCategory> StoreBranchProductCategories { get; set; }

        public virtual ICollection<PromotionProductCategory> PromotionProductCategories { get; set; }

        public virtual ICollection<ComboProductGroup> ComboProductGroups { get; set; }
    }
}

using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreBranchProductCategory))]
    public class StoreBranchProductCategory : BaseAuditEntity
    {
        [Key]
        public Guid ProductCategoryId { get; set; }

        [Key]
        public Guid StoreBranchId { get; set; }

        public Guid? StoreId { get; set; }


        public virtual ProductCategory ProductCategory { get; set; }

        public virtual StoreBranch StoreBranch { get; set; }
    }
}

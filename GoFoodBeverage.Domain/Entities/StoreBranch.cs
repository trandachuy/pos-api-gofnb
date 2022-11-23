using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreBranch))]
    public class StoreBranch : BaseEntity
    {
        public Guid? AddressId { get; set; }

        public Guid StoreId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        public int StatusId { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public virtual Store Store { get; set; }

        public virtual Address Address { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<GroupPermissionBranch> GroupPermissionBranches { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public virtual ICollection<StoreBranchProductCategory> StoreBranchProductCategories { get; set; }

        public virtual ICollection<PromotionBranch> PromotionBranches { get; set; }

        public virtual ICollection<ComboStoreBranch> ComboStoreBranches { get; set; }

        public virtual ICollection<FeeBranch> FeeBranches { get; set; }

        public virtual ICollection<MaterialInventoryChecking> MaterialInventoryCheckings { get; set; }
    }
}
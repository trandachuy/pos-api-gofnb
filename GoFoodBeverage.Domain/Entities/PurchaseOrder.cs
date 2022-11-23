using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PurchaseOrder))]
    public class PurchaseOrder : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? SupplierId { get; set; }

        public Guid? StoreBranchId { get; set; }

        public Guid? PurchaseOrderMaterialId { get; set; }

        [MaxLength(15)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Note { get; set; }

        public EnumPurchaseOrderStatus StatusId { get; set; }

        /// <summary>
        /// This field stored all related data to this material
        /// </summary>
        public string RestoreData { get; set; }

        public virtual StoreBranch StoreBranch { get; set; }

        public virtual Store Store { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<PurchaseOrderMaterial> PurchaseOrderMaterials { get; set; }
    }
}

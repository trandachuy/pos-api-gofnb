using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(MaterialInventoryHistory))]
    public class MaterialInventoryHistory : BaseEntity
    {
        public decimal OldQuantity { get; set; }

        public decimal NewQuantity { get; set; }

        public string Reference { get; set; }

        public EnumInventoryHistoryAction Action { get; set; }

        public string Note { get; set; }

        public string CreatedBy { get; set; }

        public Guid? MaterialInventoryBranchId { get; set; }

        public Guid? OrderId { get; set; }

        public virtual MaterialInventoryBranch MaterialInventoryBranch { get; set; }
    }
}

using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Area))]
    public class Area : BaseEntity
    {
        public Guid StoreId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Guid StoreBranchId { get; set; }

        public virtual StoreBranch StoreBranch { get; set; }

        public virtual ICollection<AreaTable> AreaTables { get; set; }
    }
}

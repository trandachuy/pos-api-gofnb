using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(AreaTable))]
    public class AreaTable : BaseEntity
    {
        public Guid? AreaId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public int NumberOfSeat { get; set; }

        public bool IsActive { get; set; }

        public Guid? StoreId { get; set; }

        public Area Area { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}

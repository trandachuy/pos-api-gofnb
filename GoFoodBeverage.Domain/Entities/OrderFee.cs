using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderFee))]
    public class OrderFee : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid FeeId { get; set; }

        public bool IsPercentage { get; set; }

        public decimal FeeValue { get; set; }

        //This field will be removed after handle order completed status. Get backup data from RestoreData of Order
        public string FeeName { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Fee Fee { get; set; }

        public virtual Order Order { get; set; }
    }
}

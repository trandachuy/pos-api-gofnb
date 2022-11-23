using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(CustomerMembershipLevel))]
    public class CustomerMembershipLevel : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public string Name { get; set; }

        public int AccumulatedPoint { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; }

        public decimal? MaximumDiscount {get; set;}

        public string Color { get; set; }

        public string Thumbnail { get; set; }
    }
}

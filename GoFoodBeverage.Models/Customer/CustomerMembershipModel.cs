using System;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerMembershipModel
    {
        public Guid? Id { get; set; }

        public int No { get; set; }

        public int Member { get; set; }

        public string Name { get; set; }

        public int AccumulatedPoint { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; }

        public int? MaximumDiscount { get; set; }

        public string Thumbnail { get; set; }
    }
}

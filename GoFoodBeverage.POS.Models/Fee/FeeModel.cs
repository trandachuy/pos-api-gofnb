using System;

namespace GoFoodBeverage.POS.Models.Fee
{
    public class FeeModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPercentage { get; set; }

        public decimal Value { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsStopped { get; set; }

        public bool IsAutoApplied { get; set; }
    }
}

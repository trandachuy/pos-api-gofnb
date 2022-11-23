using System;

namespace GoFoodBeverage.Models.Fee
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

        public bool IsStopped { get; set; }

        public int StatusId { get; set; }

        public bool IsAutoApplied { get; set; }
    }
}

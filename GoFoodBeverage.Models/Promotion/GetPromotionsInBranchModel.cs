using System;

namespace GoFoodBeverage.Models.Promotion
{
    public class GetPromotionsInBranchModel
    { 
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public string Name { get; set; }

        public int PromotionTypeId { get; set; }

        public bool IsPercentDiscount { get; set; }

        public decimal PercentNumber { get; set; }

        public decimal MaximumDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}

using System;

namespace GoFoodBeverage.POS.Models.Fee
{
    public class CustomerForOrderDetailModel
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string MemberShip { get; set; }

        public decimal Discount { get; set; }

        public decimal? MaximumDiscount { get; set; }

        public string Thumbnail { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderDetailModel
    {
        public Guid Id { get; set; }

        public List<PosOrderDetailProductModel> PosOrderDetailProducts { get; set; }

        public decimal? PromotionTotal { get; set; }

        public decimal? CustomerTotal { get; set; }

        public decimal? FeeTotal { get; set; }

        public decimal? TotalAmount { get; set; }

        public OrderDetailPromotion OrderDetailPromotion { get; set; }


        public OrderDetailCustomer OrderDetailCustomer { get; set; }


        public List<OrderDetailFee>  OrderDetailFees { get; set; }

        public string OrderType { get; set; }

        public DateTime? OrderTime { get; set; }

        public int OrderTotalItems { get; set; }

        public string Area { get; set; }

        public string Table { get; set; }

        public string OrderCode { get; set; }
    }

    public class PosOrderDetailProductModel
    {
        public string ProductName { get; set; }

        public string ProductPriceName  { get; set; }

        public decimal ProductPriceValue { get; set; }

        public int Quantity { get; set; }

        public List<ProductDetailOption> ProductDetailOptions { get; set; }

        public List<ProductDetailTopping> ProductDetailToppings { get; set; }

        public decimal TotalNotDiscount { get; set; }

        public decimal? Total { get; set; }

        public decimal? PromotionDiscountValue { get; set; }

        public bool? IsPromotionDiscountPercentage { get; set; }

        public string PromotionName { get; set; }
    }

    public class ProductDetailOption
    {
        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }
    }


    public class ProductDetailTopping
    {
        public string ToppingName { get; set; }

        public decimal ToppingValue { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderDetailCustomer
    {
        public string CustomerName { get; set; }

        public Guid? CustomerId { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerRank { get; set; }

        public int CustomerDiscount { get; set; }
    }

    public class OrderDetailPromotion
    {
        public string PromotionName { get; set; }

        public bool? Ispercent { get; set; }

        public decimal? PromotionDiscountValue { get; set; }
    }

    public class OrderDetailFee
    {
        public string FeeName { get; set; }

        public bool Ispercent { get; set; }

        public decimal FeeDiscountValue { get; set; }

        public decimal FeeAmount { get; set; }
    }
}

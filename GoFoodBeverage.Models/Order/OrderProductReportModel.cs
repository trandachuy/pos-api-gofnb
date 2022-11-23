
namespace GoFoodBeverage.Models.Order
{
    public class ProductRevenueReportModel
    {
        public ProductCostReportModel ProductCostReport { get; set; }

        public OrderProductReportModel OrderProductReport { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public double ProfitPercentage { get; set; }
    }

    public class ProductCostReportModel
    {
        /// <summary>
        /// If total cost > 0 => IsIncrease
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Percentage compared with the previous session
        /// </summary>
        public double Percentage { get; set; }
    }

    public class OrderProductReportModel
    {
        public double AverageOrder { get; set; }

        public int TotalOrder { get; set; }

        /// <summary>
        /// If TotalSoldItems > 0 => IsIncrease
        /// </summary>
        public int TotalSoldItems {get;set;}

        /// <summary>
        /// Percentage compared with the previous session
        /// </summary>
        public double Percentage { get; set; }
    }
}

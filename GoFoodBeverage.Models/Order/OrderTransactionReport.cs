
namespace GoFoodBeverage.Models.Order
{
    public class OrderTransactionReport
    {
        public int TotalOrderCurrent { get; set; }

        public double TotalOrderPercent { get; set; }

        public bool TotalOrderIncrease { get; set; }

        public int TotalOrderCancelCurrent { get; set; }

        public double TotalOrderCancelPercent { get; set; }

        public bool TotalOrderCancelIncrease { get; set; }

        public int TotalOrderInstoreCurrent { get; set; }

        public double TotalOrderInstorePercent { get; set; }

        public bool TotalOrderInstoreIncrease { get; set; }

        public int TotalOrderTakeAwayCurrent { get; set; }

        public double TotalOrderTakeAwayPercent { get; set; }

        public bool TotalOrderTakeAwayIncrease { get; set; }

        public int TotalOrderGoFnBAppCurrent { get; set; }

        public double TotalOrderGoFnBAppPercent { get; set; }

        public bool TotalOrderGoFnBAppIncrease { get; set; }

        public int TotalOrderStoreWebCurrent { get; set; }

        public double TotalOrderStoreWebPercent { get; set; }

        public bool TotalOrderStoreWebIncrease { get; set; }

        public int TotalOrderStoreAppCurrent { get; set; }

        public double TotalOrderStoreAppPercent { get; set; }

        public bool TotalOrderStoreAppIncrease { get; set; }
    }
}

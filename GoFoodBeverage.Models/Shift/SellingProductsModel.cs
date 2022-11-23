using GoFoodBeverage.Models.Order;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Shift
{
    public class SellingProductsModel
    {
        public List<SellingProductTableModel> SellingProductTable { get; set; }

        public int TotalQuantity { get; set; }
    }

    public class SellingProductTableModel
    {
        public int No { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}

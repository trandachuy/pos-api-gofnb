using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Shift
{
    public class ShiftModel
    {
        public int OrderValue { get; set; }

        public double? OrderPercent { get; set; }

        public bool? OrderIncrease { get; set; }

        public int SoldProductValue { get; set; }

        public double? SoldProductPercent { get; set; }

        public bool? SoldProductIncrease { get; set; }

        public decimal TotalDiscountValue { get; set; }

        public double? TotalDiscountPercent { get; set; }

        public bool? TotalDiscountIncrease { get; set; }

        public decimal RevenueValue { get; set; }

        public double? RevenuePercent { get; set; }

        public bool? RevenueIncrease { get; set; }

        public List<ShiftTableModel> ShiftTableModels { get; set; }
    }

    public class ShiftTableModel
    {
        public int No { get; set; }

        public Guid ShiftId { get; set; }

        public string StaffName { get; set; }

        public string CheckIn { get; set; }

        public string CheckOut { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal Revenue { get; set; }

        public decimal Discount { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public decimal Remain { get; set; }

        public decimal Cash { get; set; }

        public decimal MoMo { get; set; }

        public decimal ATM { get; set; }

        public int CancleOrderAmount { get; set; }
    }
}

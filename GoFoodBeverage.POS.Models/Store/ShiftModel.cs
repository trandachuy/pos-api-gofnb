using System;

namespace GoFoodBeverage.POS.Models.Store
{
    public class ShiftModel
    {
        public Guid Id { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? StaffId { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }
    }
}

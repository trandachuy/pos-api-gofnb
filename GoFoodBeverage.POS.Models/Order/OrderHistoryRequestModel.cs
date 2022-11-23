using GoFoodBeverage.Domain.Enums;
using System;

namespace GoFoodBeverage.POS.Models.Order
{
    public class OrderHistoryRequestModel
    {
        public Guid OrderId { get; set; }

        public string OldOrder { get; set; }

        public string NewOrder { get; set; }

        public EnumOrderHistoryActionName Action { get; set; }

        public string ActionName { get; set; }

        public string Note { get; set; }

        public DateTime? ActionDate { get; set; }

        public Guid? ActionBy { get; set; }
    }
}

using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoFoodBeverage.POS.Models.Area
{
    public class AreaTablesByBranchIdModel
    {

        public Guid Id { get; set; }

        public string Name { get; set; }
        

        public IEnumerable<AreaTableDto> AreaTables { get; set; }

        public int TotalTable
        {
            get
            {
                return AreaTables.Sum(m => m.NumberOfSeat);
            }
        }

        public int TotalAvailableTable
        {
            get
            {
                return TotalTable - AreaTables.Where(m => m.Orders.Count() > 0).Sum(m => m.NumberOfSeat);
            }
        }

        public class AreaTableDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int NumberOfSeat { get; set; }

            public string NumberOfStep { get; set; }

            public string Time { get; set; }

            public DateTime? OrderCreateDate { get; set; }

            public decimal PriceAfterDiscount { get; set; }

            public bool IsActive { get; set; }

            public IEnumerable<OrderDto> Orders { get; set; }

            public class OrderDto
            {
                public Guid Id { get; set; }

                public Guid? StoreId { get; set; }

                public Guid? BranchId { get; set; }

                public Guid? ShiftId { get; set; }

                public Guid? CustomerId { get; set; }

                public DateTime? CreatedTime { get; set; }

                public string Code { get; set; }

                public EnumOrderStatus StatusId { get; set; }

                public EnumOrderPaymentStatus? OrderPaymentStatusId { get; set; }

                public EnumOrderType OrderTypeId { get; set; }

                public decimal OriginalPrice { get; set; }

                public decimal TotalDiscountAmount { get; set; }

            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Stamp
{
    public class PrintStampDataModel
    {
        /// <summary>
        /// Order string code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Store's logo url
        /// </summary>
        public string Logo { get; set; }

        public DateTime CreatedTime { get; set; }

        public List<StampOrderItemDto> ItemList { get; set; }

        public class StampOrderItemDto
        {
            /// <summary>
            /// Index of item in list
            /// </summary>
            public int No { get; set; }

            /// <summary>
            /// Name of order item
            /// </summary>
            public string Name { get; set; }

            public string Note { get; set; }

            /// <summary>
            /// Current content in the stamp
            /// </summary>
            public bool Current { get; set; }

            /// <summary>
            /// List options and topping for this order item
            /// </summary>
            public List<OrderItemOptionDetailDto> Options { get; set; }
        }

        public class OrderItemOptionDetailDto
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Product
{
    public class PreventDeleteProductModel
    {
        public bool IsPreventDelete { get; set; } = false;

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// 0 : Product is in Order not complete
        /// 1 : Product is in Combo actived
        /// 2 : Product is in Promotion actived
        /// </summary>
        public int ReasonType { get; set; }

        public List<Reason> Reasons { get; set; }

        public class Reason
        {
            public Guid ReasonId { get; set; }
            public string ReasonName { get; set; }
        }
    }
}

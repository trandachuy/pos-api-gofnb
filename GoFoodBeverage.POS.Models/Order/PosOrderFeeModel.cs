using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderFeeModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid FeeId { get; set; }

        public bool IsPercentage { get; set; }

        public decimal FeeValue { get; set; }

        public string FeeName { get; set; }
    }
}

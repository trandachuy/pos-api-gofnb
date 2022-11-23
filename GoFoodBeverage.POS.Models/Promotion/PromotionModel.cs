using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Promotion
{
    public class PromotionModel
    {
        public string Name { get; set; }

        public bool IsPercentDiscount { get; set; }

        public decimal PercentNumber { get; set; }

        public decimal MaximumDiscountAmount { get; set; }
    }
}

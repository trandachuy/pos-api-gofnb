using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductToppingModel
    {
        public Guid ToppingId { get; set; }

        public string Name { get; set; }

        public decimal PriceValue { get; set; }

        public int Quantity { get; set; }
    }
}

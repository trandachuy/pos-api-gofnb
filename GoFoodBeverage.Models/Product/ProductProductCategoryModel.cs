using GoFoodBeverage.Models.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Product
{
    public class ProductProductCategoryModel
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }        

        public string UnitName { get; set; }

        public int Index { get; set; }
    }
}

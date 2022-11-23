using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Product
{
    public class ProductCategoryByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public bool IsDisplayAllBranches { get; set; } = true;

        public List<ProductSelectedModel> Products { get; set; }

        public List<Guid> StoreBranchIds { get; set; }

        public class ProductSelectedModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Position { get; set; }

            public string Thumbnail { get; set; }
        }
    }
}

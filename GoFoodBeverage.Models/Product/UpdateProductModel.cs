using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Product
{
    public class UpdateProductModel
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public Guid UnitId { get; set; }

        public Guid? TaxId { get; set; }

        public List<PriceDto> Prices { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public class PriceDto
        {
            public string Name { get; set; }

            public decimal Price { get; set; }

            public Guid? Id { get; set; }

            public List<MaterialDto> Materials { get; set; }
        }

        public List<MaterialDto> Materials { get; set; }

        public List<Guid> OptionIds { get; set; }

        public bool IsTopping { get; set; }

        public class MaterialDto
        {
            public Guid MaterialId { get; set; }

            public int Quantity { get; set; }

            public decimal UnitCost { get; set; }
        }

        public List<Guid> PlatformIds { get; set; }
    }
}

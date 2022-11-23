using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Material
{
    public class ExportMaterialModel
    {
        public Guid Id { get; set; }

        public int Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MaterialCategoryName { get; set; }

        public IEnumerable<MaterialInventoryBranchDto> MaterialInventoryBranches { get; set; }

        public class MaterialInventoryBranchDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public decimal? Cost { get; set; }

        public decimal? CostPerUnit { get; set; }

        public string UnitName { get; set; }

        public bool IsActive { get; set; }

        public int? MinQuantity { get; set; }
    }
}

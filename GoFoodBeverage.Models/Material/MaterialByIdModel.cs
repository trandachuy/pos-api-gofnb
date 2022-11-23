using GoFoodBeverage.Models.Unit;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Material
{
    public class MaterialByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public decimal? Cost { get; set; }

        public decimal? CostPerUnit { get; set; }

        public string UnitName { get; set; }

        public bool HasExpiryDate { get; set; }

        public int? MinQuantity { get; set; }

        public bool IsActive { get; set; }

        public string Thumbnail { get; set; }

        public MaterialCategoryDto MaterialCategory { get; set; }

        public UnitDto Unit { get; set; }

        public IEnumerable<MaterialInventoryBranchDto> MaterialInventoryBranches { get; set; }

        public class MaterialCategoryDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class UnitDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class MaterialInventoryBranchDto
        {
            public Guid Id { get; set; }

            public int Position { get; set; }

            public int Quantity { get; set; }

            public StoreBranchDto Branch { get; set; }

            public class StoreBranchDto
            {
                public Guid Id { get; set; }

                public string Name { get; set; }
            }
        }
    }
}

using System;

namespace GoFoodBeverage.Models.Material
{
    public class MaterialCategoryDatatableModel
    {
        public int No { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int TotalMaterial { get; set; } /// Counting MaterialCategoryMaterial records
    }
}

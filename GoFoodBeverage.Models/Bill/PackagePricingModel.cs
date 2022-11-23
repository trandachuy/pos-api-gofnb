using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Bill
{
    public class PackageModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? CostPerMonth { get; set; }

        public int Tax { get; set; }

        public bool IsActive { get; set; }

        public virtual List<FunctionModel> Functions { get; set; }
    }

    public class FunctionGroupModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual List<FunctionModel> Functions { get; set; }
    }

    public class FunctionModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}

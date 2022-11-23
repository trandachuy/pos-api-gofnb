using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Store
{
    public class StoresModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public double Rating { get; set; }

        public double Distance { get; set; }

        public bool IsPromotion { get; set; }

        public string Logo { get; set; }

        public BranchModel StoreBranches { get; set; }

        public class BranchModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }

}

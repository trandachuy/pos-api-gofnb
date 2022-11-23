using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Fee
{
    public class FeeDetailModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShowAllBranches { get; set; }

        public bool IsPercentage { get; set; }

        public decimal Value { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<StoreBranchDto> FeeBranches { get; set; }

        public class StoreBranchDto
        {
            public int Code { get; set; }

            public string Name { get; set; }
        }

        public bool IsAutoApplied { get; set; }

        public IEnumerable<ServingTypeDto> ServingTypes { get; set; }

        public class ServingTypeDto
        {
            public EnumOrderType Code { get; set; }

            public string Name { get; set; }
        }
    }
}
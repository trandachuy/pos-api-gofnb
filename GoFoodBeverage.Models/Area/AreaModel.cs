using System;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Models.Area
{
    public class AreaModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Guid StoreBranchId { get; set; }

        public virtual StoreBranch StoreBranch { get; set; }
    }
}

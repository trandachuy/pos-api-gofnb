using System;

namespace GoFoodBeverage.POS.Models.Store
{
    public class StoreBranchModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public string StoreName { get; set; }

        public string BranchName { get; set; }
    }
}

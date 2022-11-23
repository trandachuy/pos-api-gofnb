using System;

namespace GoFoodBeverage.Models.FavoriteStore
{
    public class FavoriteStoreModel
    {
        public Guid StoreId { get; set; }

        public string StoreTitle { get; set; }

        public string StoreThumbnail { get; set; }

        public bool IsPromotion { get; set; }

        public double Rating { get; set; }

        public double Distance { get; set; }

        public BranchModel StoreBranches { get; set; }

        public class BranchModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}

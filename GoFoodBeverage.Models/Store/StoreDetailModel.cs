using System;

namespace GoFoodBeverage.Models.Store
{
    public class StoreDetailModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string BranchName { get; set; }

        public string CurrencySymbol { get; set; }

        public bool IsFavoriteStore { get; set; } = false;

        public string AddressInfo { get; set; }

        public string Thumbnail { get; set; }

        public double Distance { get; set; }
    }
}

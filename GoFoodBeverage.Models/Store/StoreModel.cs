using System;
using System.Collections.Generic;
using GoFoodBeverage.Models.Address;

namespace GoFoodBeverage.Models.Store
{
    public class StoreModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid BusinessAreaId { get; set; }

        public string Code { get; set; }

        public Guid InitialStoreAccountId { get; set; }

        public Guid AddressId { get; set; }

        public double Rating { get; set; }

        public double Distance { get; set; }

        public bool IsPromotion { get; set; }

        public bool IsStoreHasKitchen { get; set; }

        public bool IsAutoPrintStamp { get; set; }

        public bool IsPaymentLater { get; set; }

        public bool IsCheckProductSell { get; set; }

        public List<BranchModel> StoreBranches { get; set; }

        public class BranchModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public virtual AddressModel Address { get; set; }

        public CurrencyModel Currency { get; set; }
    }
}

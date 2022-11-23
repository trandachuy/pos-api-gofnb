using System;

namespace GoFoodBeverage.POS.Models.Store
{
    public class StoreByAccountIdModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public Guid InitialStoreAccountId { get; set; }

        public Guid LoginAccountId { get; set; }
    }
}

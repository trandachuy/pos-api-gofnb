using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(LoyaltyPointConfig))]
    public class LoyaltyPointConfig: BaseEntity
    {
        public Guid? StoreId { get; set; }

        public bool IsActivated { get; set; }

        public bool IsExpiryDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? EarningPointExchangeValue { get; set; }

        public decimal? RedeemPointExchangeValue { get; set; }

        public bool IsExpiryMembershipDate { get; set; }

        public DateTime? ExpiryMembershipDate { get; set; }
    }
}

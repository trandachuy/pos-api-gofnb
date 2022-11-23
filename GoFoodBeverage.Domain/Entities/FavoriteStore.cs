using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FavoriteStore))]
    public class FavoriteStore : BaseEntity
    {
        public Guid StoreId { get; set; }

        public Guid? AccountId { get; set; }

        public virtual Store Store { get; set; }

        public virtual Account Account { get; set; }
    }
}

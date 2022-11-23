using GoFoodBeverage.Domain.Base;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(AccountSearchHistory))]
    public class AccountSearchHistory : BaseEntity
    {
        public Guid AccountId { get; set; }

        [MaxLength(255)]
        public string KeySearch { get; set; }

        public virtual Account Account { get; set; }
    }
}
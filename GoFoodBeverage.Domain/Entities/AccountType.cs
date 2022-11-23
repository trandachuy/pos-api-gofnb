using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(AccountType))]
    public class AccountType : BaseEntity
    {
        /// <summary>
        /// Mapping to EAccountType class
        /// </summary>
        public int EnumValue { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }


        public virtual ICollection<Account> Accounts { get; set; }
    }
}

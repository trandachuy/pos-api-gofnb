using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(AccountTransfer))]
    public class AccountTransfer
    {
        public Guid Id { get; set; }

        public string AccountOwner { get; set; }

        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string Branch { get; set; }

        public string Content { get; set; }

        public Guid? StoreId { get; set; }
    }
}

using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StoreBankAccount))]
    public class StoreBankAccount: BaseEntity
    {
        public Guid? StoreId { get; set; }

        public string SwiftCode { get; set; }

        public string RoutingNumber { get; set; }

        public string AccountHolder { get; set; }

        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string BankBranchName { get; set; }

        public Guid CountryId { get; set; }

        public Guid? CityId { get; set; }

        public virtual Store Store { get; set; }

        public virtual Country Country { get; set; }

        public virtual City City { get; set; }
    }
}

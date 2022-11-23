using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(BankAccount))]
    public class BankAccount : BaseEntity
    {
        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string TaxCode { get; set; }

        [MaxLength(100)]
        public string AccountHolder { get; set; }

        [MaxLength(20)]
        public string AccountNumber { get; set; }

        [MaxLength(100)]
        public string BankName { get; set; }

        public int CountryId { get; set; }

        public int CityId { get; set; }

        [MaxLength(100)]
        public string BranchName { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Country Country { get; set; }

        public virtual City City { get; set; }
    }
}

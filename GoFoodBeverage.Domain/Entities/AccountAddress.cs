using System;
using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(AccountAddress))]
    public class AccountAddress : BaseEntity
    {
        public Guid AccountId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string AddressDetail { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        [MaxLength(255)]
        public string Note { get; set; }

        public EnumCustomerAddressType CustomerAddressTypeId { get; set; }

        public int Possion { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Account Account { get; set; }
    }
}

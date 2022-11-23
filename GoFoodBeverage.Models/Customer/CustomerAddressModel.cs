using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerAddressModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public string AddressDetail { get; set; }

        public string Note { get; set; }

        public EnumCustomerAddressType CustomerAddressTypeId { get; set; }

        public int? Possion { get; set; }
    }
}

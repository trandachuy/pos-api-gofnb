using GoFoodBeverage.POS.Models.Address;
using System;

namespace GoFoodBeverage.POS.Models.Customer
{
    public class CustomerEditModel
    {
        public Guid Id { get; set; }

        public int Code { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Thumbnail { get; set; }

        public bool Gender { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public string Rank { get; set; }

        public decimal TotalMoney { get; set; }

        public decimal TotalOrder { get; set; }

        public virtual AddressModel Address { get; set; }
    }
}

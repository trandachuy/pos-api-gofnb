using System;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomerEditModel
    {
        public int Code { get; set; }

        public string FullName { get; set; }

        public string PlatformName { get; set; }

        public bool Gender { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? Birthday { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public Guid? AddressId { get; set; }

        public CustomerAddress CustomerAddress { get; set; }

        public string Rank { get; set; }

        public string BadgeColor { get; set; }

        public string Thumbnail { get; set; }

        public decimal TotalMoney { get; set; }

        public decimal TotalOrder { get; set; }
    }

    public class CustomerAddress
    {
        public Guid? CityId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? WardId { get; set; }

        public string Address1 { get; set; }

        public Guid? CountryId { get; set; }

        public Guid? StateId { get; set; }

        public string CityTown { get; set; }

        public string Address2 { get; set; }
    }
}

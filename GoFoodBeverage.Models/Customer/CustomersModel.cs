using System;

namespace GoFoodBeverage.Models.Customer
{
    public class CustomersModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneCode { get; set; }

        public string Email { get; set; }

        public string Rank { get; set; }

        public decimal? Point { get; set; }

        public int No { get; set; }

        public int? AccumulatedPoint { get; set; }

        public string Color { get; set; }

        public string CountryCode { get; set; }

        public Guid? CountryId { get; set; }

        public string Thumbnail { get; set; }

    }
}

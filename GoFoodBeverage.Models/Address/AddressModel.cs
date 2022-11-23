
using System;

namespace GoFoodBeverage.Models.Address
{
    public class AddressModel
    {
        public Guid? CountryId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string CityTown { get; set; }

        public CountryModel Country { get; set; }

        public StateModel State { get; set; }

        public CityModel City { get; set; }

        public DistrictModel District { get; set; }

        public WardModel Ward { get; set; }

        public string PostalCode { get; set; }
    }
}
